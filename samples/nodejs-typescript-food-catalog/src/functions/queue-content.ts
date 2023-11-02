import { InvocationContext, app } from "@azure/functions";
import { ContentMessage, CrawlType, ItemAction } from "../common/ContentMessage";
import { config } from "../common/config";
import { client } from "../common/graphClient";
import { enqueueItemDeletion, enqueueItemUpdate } from "../common/queueClient";
import { addItemToTable, getItemIds, getLastModified, recordLastModified, removeItemFromTable } from "../common/tableClient";

const { notificationEndpoint: apiUrl } = config;

async function crawl(crawlType: CrawlType, context: InvocationContext) {
    switch (crawlType) {
        case 'full':
        case 'incremental':
            await crawlFullOrIncremental(crawlType, context);
            break;
        case 'removeDeleted':
            await removeDeleted(context);
            break;
    }
}

async function crawlFullOrIncremental(crawlType: CrawlType, context: InvocationContext) {
    let url = `${apiUrl}/api/products`;

    if (crawlType === 'incremental') {
        const lastModified = await getLastModified(context);
        url += `?$filter=last_modified_t gt ${lastModified}`;
    }

    context.log(`Retrieving items from ${url}...`);

    const res = await fetch(url);
    if (!res.ok) {
        context.log(`Error retrieving item from ${url}: ${res.statusText}`);
        return;
    }

    const products: Product[] = await res.json();
    context.log(`Retrieved ${products.length} items from ${url}`);

    for (const product of products) {
        context.log(`Enqueuing item update for ${product.id}...`);
        enqueueItemUpdate(product.id);
    }
}

async function removeDeleted(context: InvocationContext) {
    const url = `${apiUrl}/api/products`;

    context.log(`Retrieving items from ${url}...`);

    const res = await fetch(url);
    if (!res.ok) {
        context.log(`Error retrieving item from ${url}: ${res.statusText}`);
        return;
    }

    const products: Product[] = await res.json();
    context.log(`Retrieved ${products.length} items from ${url}`);

    context.log('Retrieving ingested items...');
    const ingestedItemIds = await getItemIds(context);

    ingestedItemIds.forEach(ingestedItemId => {
        if (products.find(product => product.id === ingestedItemId)) {
            context.log(`Item ${ingestedItemId} still exists, skipping...`);
        }
        else {
            context.log(`Item ${ingestedItemId} no longer exists, deleting...`);
            enqueueItemDeletion(ingestedItemId);
        }
    });
}

async function processItem(itemId: string, itemAction: ItemAction, context: InvocationContext) {
    switch (itemAction) {
        case 'update':
            await updateItem(itemId, context);
            break;
        case 'delete':
            await deleteItem(itemId, context);
            break;
    }
}

async function updateItem(itemId: string, context: InvocationContext) {
    const url = `${apiUrl}/api/products/${itemId}`;

    context.log(`Retrieving item from ${url}...`);

    const res = await fetch(url);
    if (!res.ok) {
        context.log(`Error retrieving item from ${url}: ${res.statusText}`);
        return;
    }

    const product: Product = await res.json();
    context.log(`Retrieved product from ${url}`);
    context.log(JSON.stringify(product, null, 2));

    const externalItem = {
        id: product.id,
        properties: {
            'categories@odata.type': 'Collection(String)',
            categories: product.categories?.replace(/en:/g, '')?.split(', ') ?? [''],
            ecoscore: product.ecoscore_grade,
            imageUrl: product.image_url,
            'ingredients@odata.type': 'Collection(String)',
            ingredients: product.ingredients_text?.split(', ') ?? [''],
            nutriscore: product.nutriscore_grade,
            'traces@odata.type': 'Collection(String)',
            traces: product.traces_tags?.replace(/en:/g, '')?.split(', ') ?? [''],
            name: product.product_name,
            url: product.url.replace('.net/', '.org/')
        },
        content: {
            value: product.product_name,
            type: 'text'
        },
        acl: [
            {
                accessType: 'grant',
                type: 'everyone',
                value: 'everyone'
            }
        ]
    }

    context.log(`Transformed item`);
    context.log(JSON.stringify(externalItem, null, 2));

    const externalItemUrl = `/external/connections/${config.connector.id}/items/${product.id}`;
    context.log(`Updating external item ${externalItemUrl}...`)

    await client
        .api(externalItemUrl)
        .header('content-type', 'application/json')
        .put(externalItem);

    context.log(`Adding item ${product.id} to table storage...`);
    // track item to support deletion
    await addItemToTable(product.id, context);
    context.log(`Tracking last modified date ${product.last_modified_t}`);
    // track last modified date for incremental crawl
    await recordLastModified(product.last_modified_t, context);
}

async function deleteItem(itemId: string, context: InvocationContext) {
    const externalItemUrl = `/external/connections/${config.connector.id}/items/${itemId}`;
    context.log(`Deleting external item ${externalItemUrl}...`)

    await client
        .api(externalItemUrl)
        .delete();

    context.log(`Removing item ${itemId} from table storage...`);
    await removeItemFromTable(itemId, context);
}

app.storageQueue("contentQueue", {
    connection: "AzureWebJobsStorage",
    queueName: "queue-content",
    handler: async (message: ContentMessage, context: InvocationContext) => {
        context.log('Received message from queue queue-content');
        context.log(JSON.stringify(message, null, 2));

        const { action, crawlType, itemAction, itemId } = message;

        switch (action) {
            case 'crawl':
                await crawl(crawlType, context);
                break;
            case 'item':
                await processItem(itemId, itemAction, context);
                break;
            default:
                break;
        }
    }
});
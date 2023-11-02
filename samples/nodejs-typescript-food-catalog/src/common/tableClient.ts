import { TableClient, TableServiceClient } from "@azure/data-tables";
import { InvocationContext } from "@azure/functions";
import { config } from "./config";

interface StateRecord {
    date: number;
}

export async function getTableClient(tableName: string) {
    const { storageAccountConnectionString } = config;
    const tableServiceClient = TableServiceClient.fromConnectionString(storageAccountConnectionString);
    await tableServiceClient.createTable(tableName);
    return TableClient.fromConnectionString(storageAccountConnectionString, tableName);
}

export async function getItemIds(context: InvocationContext) {
    context.log(`Getting table client for externalitems...`);
    const tableClient = await getTableClient('externalitems');
    const entities = tableClient.listEntities();

    const items: string[] = [];
    for await (const entity of entities) {
        items.push(entity.rowKey);
    }

    return items;
}

export async function addItemToTable(itemId: string, context: InvocationContext) {
    context.log(`Getting table client for externalitems...`);
    const tableClient = await getTableClient('externalitems');
    const entity = {
        partitionKey: 'products',
        rowKey: itemId
    }

    context.log(`Upserting entity ${JSON.stringify(entity, null, 2)}...`);
    await tableClient.upsertEntity(entity);
}

export async function removeItemFromTable(itemId: string, context: InvocationContext) {
    context.log(`Getting table client for externalitems...`);
    const tableClient = await getTableClient('externalitems');
    context.log(`Deleting entity ${itemId}...`);
    await tableClient.deleteEntity('products', itemId);
}

export async function recordLastModified(lastModifiedDate: number, context: InvocationContext) {
    context.log(`Getting table client for state...`);
    const tableClient = await getTableClient('state');
    let lastModified
    try {
        context.log(`Getting entity lastModified...`);
        lastModified = await tableClient.getEntity<StateRecord>('state', 'lastModified');
    }
    catch (e) {
        context.log(`Error getting entity lastModified: ${e.message}`);
    }
    if (lastModified && lastModified.date > lastModifiedDate) {
        context.log(`Last modified date ${lastModified.date} is newer than ${lastModifiedDate}`);
        // we've got a newer record already
        return;
    }

    const entity = {
        partitionKey: 'state',
        rowKey: 'lastModified',
        date: lastModifiedDate
    };
    context.log(`Upserting entity ${JSON.stringify(entity, null, 2)}...`);
    await tableClient.upsertEntity(entity);
}

export async function getLastModified(context: InvocationContext) {
    context.log(`Getting table client for state...`);
    const tableClient = await getTableClient('state');
    let lastModified
    try {
        context.log(`Getting entity lastModified...`);
        lastModified = await tableClient.getEntity<StateRecord>('state', 'lastModified');
        context.log(`Got lastModified: ${JSON.stringify(lastModified, null, 2)}`);
        return lastModified.date;
    }
    catch (e) {
        context.log(`Error getting entity lastModified: ${e.message}. Returning 0`);
        return 0;
    }
}
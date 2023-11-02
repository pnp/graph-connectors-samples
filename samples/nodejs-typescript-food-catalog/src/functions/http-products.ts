import { HttpRequest, InvocationContext, app } from "@azure/functions";
import { getTableClient } from "../common/tableClient";
import { randomUUID } from "crypto";
import { streamToJson } from "../common/utils";

app.http('getProducts', {
    methods: ['GET'],
    route: 'products',
    handler: async (request: HttpRequest) => {
        const filter = request.query.get('$filter');
        const select = request.query.get('$select')?.split(',') ?? ['rowKey', 'last_modified_t'];

        let products: Product[] = [];
        const tableClient = await getTableClient('products');
        const entities = tableClient.listEntities({
            queryOptions: {
                filter,
                select
            }
        });
        for await (const entity of entities) {
            const product: Product = {
                id: entity.rowKey,
                ...entity
            };
            delete (product as any).etag;
            delete (product as any).rowKey;
            products.push(product);
        }

        // it's important that the API returns products sorted
        // by last_modified_t, so that we can properly store
        // the last modified date for incremental crawls
        // this is especially important if the initial crawl breaks
        // mid-crawl and we need to resume from the last modified date
        products = products.sort((a, b) => a.last_modified_t - b.last_modified_t);

        return {
            status: 200,
            body: JSON.stringify(products, null, 2),
            headers: {
                'Content-Type': 'application/json'
            }
        }
    }
});

app.http('getProduct', {
    methods: ['GET'],
    route: 'products/{id}',
    handler: async (request: HttpRequest, context: InvocationContext) => {
        const { id } = request.params;

        try {
            const tableClient = await getTableClient('products');
            const productEntity = await tableClient.getEntity('products', id);
            delete productEntity.partitionKey;
            delete productEntity.timestamp;
            delete productEntity.etag;
            delete productEntity['odata.metadata'];

            const product: Product = {
                id: productEntity.rowKey,
                ...productEntity
            };
            delete (product as any).rowKey;
            
            return {
                status: 200,
                body: JSON.stringify(product, null, 2),
            }
        } catch (error) {
            return {
                status: error.statusCode,
            }
        }
    }
});

app.http('createProduct', {
    methods: ['POST'],
    route: 'products',
    handler: async (request: HttpRequest) => {
        const { body } = request;

        try {
            const tableClient = await getTableClient('products');
            const newProduct = {
                partitionKey: "products",
                rowKey: randomUUID().replace(/-|[a-z]/g, ''),
                last_modified_t: Date.now(),
                ...await streamToJson(body),
            }
            await tableClient.createEntity(newProduct);
            return {
                status: 201
            }
        } catch (error) {
            return {
                status: error.statusCode,
            }
        }
    }
});

app.http('updateProduct', {
    methods: ['PATCH'],
    route: 'products/{id}',
    handler: async (request: HttpRequest) => {
        const { id } = request.params;
        const { body } = request;
        try {
            const tableClient = await getTableClient('products');
            const product = await tableClient.getEntity("products", id);
            await tableClient.updateEntity({ ...product, ...await streamToJson(body), last_modified_t: Math.floor(Date.now() / 1000), }, "Merge");
            return {
                status: 200
            }
        } catch (error) {
            return {
                status: error.statusCode,
            }
        }
    }
});

app.http('deleteProduct', {
    methods: ['DELETE'],
    route: 'products/{id}',
    handler: async (request: HttpRequest) => {
        const { id } = request.params;
        try {
            const tableClient = await getTableClient('products');
            await tableClient.getEntity("products", id);
            await tableClient.deleteEntity('products', id);
            return {
                status: 200
            }
        } catch (error) {
            return {
                status: error.statusCode,
            }
        }
    }
});
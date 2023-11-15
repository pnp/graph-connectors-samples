import { app, InvocationContext } from "@azure/functions";
import { ResponseType } from "@microsoft/microsoft-graph-client";
import { ExternalConnectors } from "@microsoft/microsoft-graph-types";
import { config } from "../common/config";
import { client } from "../common/graphClient";
import { enqueueCheckStatus, startCrawl } from "../common/queueClient";
import { resultLayout } from "../common/resultLayout";
import { ConnectionMessage } from "../common/ConnectionMessage";

async function createConnection(connectorId: string, connectorTicket: string) {
    const { id, name, description, activitySettings, searchSettings } = config.connector;
    searchSettings.searchResultTemplates[0].layout = resultLayout;

    await client
        .api('/external/connections')
        .version('beta')
        .header('GraphConnectors-Ticket', connectorTicket)
        .post({
            id,
            connectorId,
            name,
            description,
            activitySettings,
            searchSettings
        });
}

async function createSchema() {
    const { id, schema } = config.connector;
    const res: Response = await client
        .api(`/external/connections/${id}/schema`)
        .responseType(ResponseType.RAW)
        .header('content-type', 'application/json')
        .patch({
            baseType: 'microsoft.graph.externalItem',
            properties: schema
        });

    const location: string = res.headers.get('Location');
    await enqueueCheckStatus(location);
}

async function checkSchemaStatus(location: string, context: InvocationContext) {
    const res: ExternalConnectors.ConnectionOperation = await client
        .api(location)
        .get();

    context.log(`Schema provisioning status: ${res.status}`);

    switch (res.status) {
        case 'inprogress':
            await enqueueCheckStatus(location);
            break;
        case 'completed':
            await startCrawl('full');
            break;
    }
}

async function deleteConnection() {
    await client.api(`/external/connections/${config.connector.id}`).delete();
}

app.storageQueue("connectionQueue", {
    connection: "AzureWebJobsStorage",
    queueName: "queue-connection",
    handler: async (message: ConnectionMessage, context: InvocationContext) => {
        context.log('Received message from queue queue-connection');
        context.log(JSON.stringify(message, null, 2));

        const { action, connectorId, connectorTicket, location } = message;

        switch (action) {
            case 'create':
                context.log('Creating connection...');
                await createConnection(connectorId, connectorTicket);
                context.log('Connection created');
                context.log('Submitting schema for provisioning...');
                createSchema();
                context.log('Schema submitted');
                break;
            case 'delete':
                context.log('Deleting connection...');
                await deleteConnection();
                context.log('Connection deleted');
                break;
            case 'status':
                context.log('Checking schema status...');
                await checkSchemaStatus(location, context);
                context.log('Schema status checked');
                break;
            default:
                break;
        }
    }
})
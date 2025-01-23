import { app, HttpRequest, HttpResponseInit, InvocationContext } from "@azure/functions";
import { Graph } from "../modules/graph";
import { config } from "../config/config";

export async function deleteConnection(request: HttpRequest, context: InvocationContext): Promise<HttpResponseInit> {
    if (!config.externalConnection.id) {
        return {
            status: 404,
            jsonBody: {
                message: "Connection ID not found in configuration"
            }
        };
    }

    const graph = new Graph();
    const existingConnection = await graph.getExternalConnection();
    if (existingConnection) {
        await graph.deleteExternalConnection();
        return {
            status: 200,
            jsonBody: {
                message: `The external connection '${graph.connectionId}' has been deleted.`
            }
        };
    } else {
        return {
            status: 404,
            jsonBody: {
                message: "Connection not found"
            }
        };
    }
};

app.http('deleteConnection', {
    route: 'deleteConnection',
    methods: ['GET'],
    authLevel: 'anonymous',
    handler: deleteConnection
});

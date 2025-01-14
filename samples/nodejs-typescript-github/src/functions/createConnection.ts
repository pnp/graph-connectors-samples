import { app, HttpRequest, HttpResponseInit, InvocationContext } from "@azure/functions";
import { config } from "../config/config";
import { Graph } from "../modules/graph";

export async function createConnection(request: HttpRequest, context: InvocationContext): Promise<HttpResponseInit> {
    if (!config.externalConnection.id) {
        return {
            status: 404,
            jsonBody: {
                message: "Connection ID not found in configuration"
            }
        };
    }

    const graph = new Graph();
    await graph.initialize();

    return {
        status: 200,
        jsonBody: {
            message: `The external connection '${graph.connectionId}' has been initialised. Please allow at least 15 minutes for the connection to be fully provisioned.`
        }
    };
};

app.http('createConnection', {
    route: 'createConnection',
    methods: ['GET'],
    authLevel: 'anonymous',
    handler: createConnection
});

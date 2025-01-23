import { Client, ClientOptions } from "@microsoft/microsoft-graph-client";
import { TokenCredentialAuthenticationProvider } from "@microsoft/microsoft-graph-client/authProviders/azureTokenCredentials";
import { ClientSecretCredential } from "@azure/identity";
import { ExternalConnectors } from "@microsoft/microsoft-graph-types";
import { config } from "../config/config";

export class Graph {

    public client: Client;
    public connectionId: string;

    constructor() {
        try {
            if (!config.externalConnection.id) {
                throw new Error("Connection ID not found in configuration");
            }
            this.connectionId = config.externalConnection.id;
            const credential = new ClientSecretCredential(
                process.env.MicrosoftAppTenantId as string,
                process.env.MicrosoftAppId as string,
                process.env.MicrosoftAppPassword as string
            );
            // Auth provider
            const authProvider = new TokenCredentialAuthenticationProvider(credential, {
                scopes: ['https://graph.microsoft.com/.default'],
            });
            const clientOptions: ClientOptions = {
                defaultVersion: "beta",
                debugLogging: false,
                authProvider
            };
            const client = Client.initWithMiddleware(clientOptions);
            this.client = client;
        } catch (error) {
            throw error;
        }
    }

    public async initialize(): Promise<void> {
        try {
            // Get the external connection
            const existingConnection = await this.getExternalConnection();
            if (!existingConnection) {
                // Create the external connection
                await this.createExternalConnection(config.externalConnection);
            }
            // Create the schema
            await this.createSchema(config.schema);
        } catch (error) {
            throw error;
        }
    }

    public async createExternalConnection(externalConnection: ExternalConnectors.ExternalConnection): Promise<void> {
        try {
            await this.client.api('/external/connections')
                .post(externalConnection);
        } catch (error) {
            throw error;
        }
    }

    public async getExternalConnection(): Promise<ExternalConnectors.ExternalConnection | undefined> {
        try {
            const result = await this.client.api(`/external/connections/${this.connectionId}`)
                .get();
            return result;
        } catch (error) {
            if (error.statusCode === 404) {
                return undefined;
            } else {
                throw error;
            }
        }
    }

    public async deleteExternalConnection(): Promise<void> {
        try {
            await this.client.api(`/external/connections/${this.connectionId}`)
                .delete();
        } catch (error) {
            throw error;
        }
    }

    public async getExternalConnectionQuota(): Promise<any> {
        try {
            const result = await this.client.api(`/external/connections/${this.connectionId}/quota`)
                .get();
            return result;
        } catch (error) {
            throw error;
        }
    }

    public async getSchema(): Promise<ExternalConnectors.Schema | undefined> {
        try {
            const result = await this.client.api(`/external/connections/${this.connectionId}/schema`)
                .get();
            return result;
        } catch (error) {
            if (error.statusCode === 404) {
                return undefined;
            } else {
                throw error;
            }
        }
    }

    public async createSchema(schema: ExternalConnectors.Schema): Promise<void> {
        try {
            await this.client.api(`/external/connections/${this.connectionId}/schema`)
                .update(schema);
        } catch (error) {
            throw error;
        }
    }

    public async getExternalItem(itemId: string): Promise<ExternalConnectors.ExternalItem | undefined> {
        try {
            const result = await this.client.api(`/external/connections/${this.connectionId}/items/${itemId}`)
                .get();
            return result;
        } catch (error) {
            if (error.statusCode === 404) {
                return undefined;
            } else {
                throw error;
            }
        }
    }

    public async putExternalItem(externalItem: ExternalConnectors.ExternalItem): Promise<void> {
        try {
            console.log(`Updating/creating external item: ${externalItem.id}`);
            await this.client.api(`/external/connections/${this.connectionId}/items/${externalItem.id}`)
                .put(externalItem);
        } catch (error) {
            throw error;
        }
    }

}
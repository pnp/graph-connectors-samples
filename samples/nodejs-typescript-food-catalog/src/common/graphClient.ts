import { ClientSecretCredential } from '@azure/identity';
import { Client, MiddlewareFactory } from '@microsoft/microsoft-graph-client';
import { TokenCredentialAuthenticationProvider } from '@microsoft/microsoft-graph-client/authProviders/azureTokenCredentials/index.js';
import { config } from './config';
import { ProxyAgent } from 'undici';

const credential = new ClientSecretCredential(
  config.entraAppTenantId,
  config.entraAppClientId,
  config.entraAppClientSecret
);

const dispatcher = process.env.https_proxy ? new ProxyAgent(process.env.https_proxy) : undefined;

const fetchOptions: any = {
  dispatcher
};

const authProvider = new TokenCredentialAuthenticationProvider(credential, {
  scopes: ['https://graph.microsoft.com/.default'],
});

const middleware = MiddlewareFactory.getDefaultMiddlewareChain(authProvider);

export const client = Client.initWithMiddleware({ middleware, fetchOptions });

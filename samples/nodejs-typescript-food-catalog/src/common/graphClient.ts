import { ClientSecretCredential } from '@azure/identity';
import { Client, MiddlewareFactory } from '@microsoft/microsoft-graph-client';
import { TokenCredentialAuthenticationProvider } from '@microsoft/microsoft-graph-client/authProviders/azureTokenCredentials/index.js';
import { config } from './config';

const credential = new ClientSecretCredential(
  config.aadAppTenantId,
  config.aadAppClientId,
  config.aadAppClientSecret
);

const authProvider = new TokenCredentialAuthenticationProvider(credential, {
  scopes: ['https://graph.microsoft.com/.default'],
});

const middleware = MiddlewareFactory.getDefaultMiddlewareChain(authProvider);

export const client = Client.initWithMiddleware({ middleware });

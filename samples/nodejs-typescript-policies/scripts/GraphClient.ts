import { ClientSecretCredential } from '@azure/identity';
import { Client, MiddlewareFactory } from '@microsoft/microsoft-graph-client';
import { TokenCredentialAuthenticationProvider } from '@microsoft/microsoft-graph-client/authProviders/azureTokenCredentials/index.js';
import { LongRunningOperationMiddleware } from './LongRunningOperationMiddleware';
import { config } from './Config';


export const initClient = (): Client => { 
  const credential = new ClientSecretCredential(
    config.tenantId,
    config.clientId,
    config.clientSecret
  );
  
  const authProvider = new TokenCredentialAuthenticationProvider(credential, {
    scopes: ['https://graph.microsoft.com/.default'],
  });
  
  const middleware = MiddlewareFactory.getDefaultMiddlewareChain(authProvider);
  // add as a second middleware to get access to the access token
  middleware.splice(1, 0, new LongRunningOperationMiddleware(30000));
  
  return Client.initWithMiddleware({ middleware }) 
}
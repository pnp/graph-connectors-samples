import { DefaultAzureCredential } from '@azure/identity';
import { Client, MiddlewareFactory } from '@microsoft/microsoft-graph-client';
import { TokenCredentialAuthenticationProvider } from '@microsoft/microsoft-graph-client/authProviders/azureTokenCredentials/index.js';
import { LongRunningOperationMiddleware } from './LongRunningOperationMiddleware';

/**
 * Initializes a new instance of the Microsoft Graph client.
 * @returns A new instance of the Microsoft Graph client.
 */
export const initClient = (): Client => { 
  const credential = new DefaultAzureCredential();
  
  const authProvider = new TokenCredentialAuthenticationProvider(credential, {
    scopes: ['https://graph.microsoft.com/.default'],
  });
  
  const middleware = MiddlewareFactory.getDefaultMiddlewareChain(authProvider);
  // add as a second middleware to get access to the access token
  middleware.splice(1, 0, new LongRunningOperationMiddleware(30000));
  
  return Client.initWithMiddleware({ middleware }) 
}
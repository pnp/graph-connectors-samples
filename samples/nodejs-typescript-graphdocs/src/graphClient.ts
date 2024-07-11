import { ClientSecretCredential } from '@azure/identity';
import { Client, MiddlewareFactory } from '@microsoft/microsoft-graph-client';
import { TokenCredentialAuthenticationProvider } from '@microsoft/microsoft-graph-client/authProviders/azureTokenCredentials/index.js';
import { ProxyAgent } from 'undici';
import { CompleteJobWithDelayMiddleware } from './completeJobWithDelayMiddleware.js';
import { appInfo } from './env.js';

const credential = new ClientSecretCredential(
  appInfo.tenantId,
  appInfo.appId,
  appInfo.secrets[0].value
);

const authProvider = new TokenCredentialAuthenticationProvider(credential, {
  scopes: ['https://graph.microsoft.com/.default'],
});

const middleware = MiddlewareFactory.getDefaultMiddlewareChain(authProvider);
// add as a second middleware to get access to the access token
middleware.splice(1, 0, new CompleteJobWithDelayMiddleware(60_000));

const dispatcher = process.env.http_proxy ? new ProxyAgent(process.env.http_proxy) : undefined;
const fetchOptions = {
  dispatcher
} as any;

export const client = Client.initWithMiddleware({ middleware, fetchOptions });

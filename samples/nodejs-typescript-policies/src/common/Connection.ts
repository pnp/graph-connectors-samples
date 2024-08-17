import { delay } from './Common';
import { config } from './Config';
import { initClient } from './GraphClient';

const { id, name, description } = config.connector;
let client = initClient();
const timeout = 600000; // 10 minutes
const retryInterval = 15000; // 15 seconds
const initialTimestamp = Date.now();
let consentRequested = false;

/**
 * Creates a connection in Microsoft Graph.
 */
async function createConnection() {  
  console.log(`Creating connection ${id}.`);

  await client
    .api('/external/connections')
    .post({
      id,
      name,
      description
    });

  console.log(`Connection ${id} was created`);
}

/**
 * Updates a connection in Microsoft Graph.
 */
export async function setSearchSettings() {  
  console.log(`Updating search settings of connection ${id}.`);

  await client
    .api(`/external/connections/${id}`)  
    .patch({
      searchSettings: {
        searchResultTemplates: [{
          id: config.connector.id,
          layout: config.connector.template,
          priority: 1
        }]
      }
    });

  console.log(`Connection ${id} was created`);
}

/**
 * Retrieves a connection from Microsoft Graph.
 * @returns The connection object.
 */
async function getConnection(): Promise<any> {
  const connection = await client
    .api(`/external/connections/${id}`)
    .get();

  return connection;
}

/**
 * Ensures that the connection exists in Microsoft Graph.
 * @returns A boolean indicating if the connection was successfully created or already exists.
 */
export async function ensureConnection(): Promise<boolean> {  
  try {
    // If time elapsed is less than 10 minutes, try again
    if(Date.now() - initialTimestamp <= timeout) {
      // We need to re-initialize the client because of granting the admin consent
      client = initClient();
      await getConnection();
      console.log(`Connection ${id} already exists`);
      return true;
    } else {
      console.error(`Could not create connection ${id} in under 10 minutes`);
    }
  } catch (e) {
    // The connection does not exist, so we need to create it
    if(e.statusCode === 404) {
      await createConnection();
      return true;
    // The authentication is failing, so we need to re-initialize the client as the developer is about grant tenant-wide admin consent
    } else if (e.statusCode === 401 || e.statusCode === 403 || (e.statusCode === -1 && e.code === "AuthenticationRequiredError")) {
      if(!consentRequested) {
        console.warn(`\nYou need to grant tenant-wide admin consent to the application in Entra ID\nClick on this link to provide the consent\nhttps://entra.microsoft.com/#view/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/~/CallAnAPI/appId/${config.clientId}/isMSAApp~/false`);
        consentRequested = true;
      }
      
      await delay(retryInterval);
      return await ensureConnection();

    } else {
      console.error(e);
    }

    return false;
  }
}
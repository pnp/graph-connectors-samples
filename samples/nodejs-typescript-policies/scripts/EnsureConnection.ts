import { delay } from './Common';
import { config } from './Config';
import { initClient } from './GraphClient';

const { id, name, description } = config.connector;
let client = initClient();
const timeout = 600000; // 10 minutes
const retryInterval = 15000; // 15 seconds
const initialTimestamp = Date.now();
let consentRequested = false;

async function createConnection() {  
  if(config.debug) {
    console.log(`POST: /external/connections`);
    console.log(`Connection: ${JSON.stringify({
      id,
      name,
      description
    }, null, 2)}`);
  }

  console.log(`Creating connection ${id}. This should take under 10 minutes...`);

  await client
    .api('/external/connections')
    .post({
      id,
      name,
      description
    });

  console.log(`Connection ${id} was created`);
}

async function getConnection(): Promise<any> {
  if(config.debug) {
    console.log(`GET: /external/connections/${id}`);
  }

  const connection = await client
    .api(`/external/connections/${id}`)
    .get();

  return connection;
}

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
      if(config.debug) {
        console.warn(e);
      }
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
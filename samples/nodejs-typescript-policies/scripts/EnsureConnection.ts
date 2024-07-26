import { config } from './Config';
import { client } from './GraphClient';

const { id, name, description } = config.connector;

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
    await getConnection();
    console.log(`Connection ${id} already exists`);
    return true;
  } catch (e) {
    if(e.statusCode === 404) {
      await createConnection();
      return true;
    } else if (e.statusCode === 401) {
      console.error(`\nYou need to grant tenant-wide admin consent to the application in Entra ID\nClick on this link to provide the consent\nhttps://entra.microsoft.com/#view/Microsoft_AAD_RegisteredApps/ApplicationMenuBlade/~/CallAnAPI/appId/${config.clientId}/isMSAApp~/false\nOnce done, please restart the "Publish" process in Teams Toolkit`);
    } else {
      console.error(e);
    }

    return false;
  }
}
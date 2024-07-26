import { config } from './Config';
import { client } from './GraphClient';

const { id, name, description } = config.connector;

async function createConnection() {  
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
    } else if (e.code === "AuthenticationRequiredError") {
      setTimeout(ensureConnection, 10000);
    } else {
      console.error(e);
    }

    return false;
  }
}
import { ResponseType } from '@microsoft/microsoft-graph-client';
import { config } from './Config';
import { client } from './GraphClient';

const { id, name, description } = config.connector;

async function createConnector() {  
  await client
    .api('/external/connections')
    .post({
      id,
      name,
      description
    });
}

async function getConnector(): Promise<any> {
  const connection = await client
    .api(`/external/connections/${id}`)
    .get();

  return connection;
}

async function ensureConnector() {  
  try {
    await getConnector();
  } catch (e) {
    if(e.statusCode === 404) {
      await createConnector();
    }
  }

  console.log(`Connection ${id} is available`);
}

async function main() {
  try {
    await ensureConnector();
  }
  catch (e) {
    console.error(e);
  }
}

main();
import { config } from './Config';
import { client } from './GraphClient';

const { id, schema } = config.connector;

async function createSchema() {
  try {
    await client
      .api(`/external/connections/${id}/schema`)
      .header('content-type', 'application/json')
      .post({
        baseType: 'microsoft.graph.externalItem',
        properties: schema
      });
  }
  catch (e) {
    console.error(e);
  }
}

async function getSchema(): Promise<any> {
  const connection = await client
    .api(`/external/connections/${id}/schema`)
    .get();

  return connection;
}

async function ensureSchema() {  
  try {
    await getSchema();
  } catch (e) {
    if(e.statusCode === 404) {
      await createSchema();
    }
  }
  
  console.log(`Schema for ${id} is available`);
}

async function main() {
  try {
    await ensureSchema();
  }
  catch (e) {
    console.error(e);
  }
}

main();
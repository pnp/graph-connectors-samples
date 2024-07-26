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
    
    console.log(`Schema for connection ${id} was created`);
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

export async function ensureSchema(): Promise<boolean> {  
  try {
    await getSchema();
    return true;
  } catch (e) {
    if(e.statusCode === 404) {
      await createSchema();
    }
  }
}
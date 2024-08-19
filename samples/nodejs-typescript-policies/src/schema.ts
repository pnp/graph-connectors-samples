import { delay } from './utils';
import { config } from './config';
import { getClient } from './graphClient';

const { id, schema } = config.connector;
const retryInterval = 15_000; // 15 seconds
const client = getClient();

/**
 * Creates a schema for the connection in Microsoft Graph.
 */
async function createSchema() {
  try {
    console.log(`Creating schema for connection ${id}. This should take under 10 minutes...`);

    await client.api(`/external/connections/${id}/schema`).header('content-type', 'application/json').post({
      baseType: 'microsoft.graph.externalItem',
      properties: schema
    });

    console.log(`Schema for connection ${id} was created`);
  } catch (e) {
    console.error(e);
  }
}

/**
 * Retrieves the schema for the connection from Microsoft Graph.
 */
async function getSchema(): Promise<any> {
  await client.api(`/external/connections/${id}/schema`).get();
}

/**
 * Ensures that the schema exists in Microsoft Graph.
 */
export async function ensureSchema() {
  try {
    // Try to get the schema
    await getSchema();
  } catch (e) {
    if (e.statusCode === 404) {
      // If the schema does not exist, create it
      await createSchema();
    } else {
      // If the error is not 404, retry
      await delay(retryInterval);
      ensureSchema();
    }
  }
}

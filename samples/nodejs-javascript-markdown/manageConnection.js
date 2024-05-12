import fs from 'fs';
import { config } from './config.js';
import { client } from './graphClient.js';

export async function createConnection(ticket) {
  console.log('Creating connection...');

  const { id, name, description, activitySettings, searchSettings } = config.connector;
  const adaptiveCard = fs.readFileSync('./resultLayout.json', 'utf8');
  searchSettings.searchResultTemplates[0].layout = JSON.parse(adaptiveCard);

  let request = client.api('/external/connections');
  if (ticket) {
    request = request.header('GraphConnectors-Ticket', ticket);
  }

  await request.post({
    id,
    name,
    description,
    activitySettings,
    searchSettings
  });

  console.log('Connection created');
}

export async function createSchema() {
  console.log('Creating schema...');

  const { id, schema } = config.connector;
  try {
    const res = await client
      .api(`/external/connections/${id}/schema`)
      .header('content-type', 'application/json')
      .patch({
        baseType: 'microsoft.graph.externalItem',
        properties: schema
      });

    const status = res.status;
    if (status === 'completed') {
      console.log('Schema created');
    }
    else {
      console.error(`Schema creation failed: ${res.error.message}`);
    }
  }
  catch (e) {
    console.error(e);
  }
}

export async function deleteConnection(ticket) {
  console.log('Deleting connection...');

  const { id } = config.connector;

  await client
    .api(`/external/connections/${id}`)
    .header('GraphConnectors-Ticket', ticket)
    .delete();

  console.log('Connection deleted');
}
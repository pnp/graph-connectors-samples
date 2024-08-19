import { ExternalConnectors } from '@microsoft/microsoft-graph-types';
import { getClient } from './graphClient';
import { Config } from './models/Config';
import { getAllItems } from './services/itemsService';
import { Item } from './models/Item';

const client = getClient();

/**
 * Transforms the content into a format that can be ingested by the Graph API.
 * @param items - The items to transform.
 * @returns An array of objects that can be ingested by the Graph API.
 */
function transformContent(items: Item[]): ExternalConnectors.ExternalItem[] {
  return items.map(item => {
    return {
      id: item.id,
      properties: {
        lastModified: item.lastModified.toISOString().slice(0, -5) + 'Z',
        title: item.title,
        abstract: item.abstract,
        author: item.author,
        url: item.url
      },
      content: {
        value: item.content,
        type: 'text'
      },
      acl: [
        {
          accessType: 'grant',
          type: 'everyone',
          value: 'everyone'
        }
      ]
    };
  });
}

/**
 * Loads the content into the Graph API.
 * @param config - The configuration object.
 * @param doc - The document to load.
 * @returns A promise that resolves when the content has been loaded.
 */
async function loadContent(config: Config, doc: ExternalConnectors.ExternalItem): Promise<void> {
  try {
    config.context.log(`Loading ${doc.id}...`);
    await client
      .api(`/external/connections/${config.connector.id}/items/${doc.id}`)
      .header('content-type', 'application/json')
      .put(doc);
  } catch (e) {
    config.context.error(`Failed to load ${doc.id}: ${e.message}`);
    if (e.body) {
      config.context.error(`${JSON.parse(e.body, null)?.innerError?.message}`);
    }
    return;
  }
}

/**
 * Ensures that the content is ingested into the Graph API.
 * @param config - The configuration object.
 */
export async function ingestContent(config: Config): Promise<void> {
  const files = await getAllItems(config);
  for (const doc of transformContent(files)) {
    await loadContent(config, doc);
  }
}

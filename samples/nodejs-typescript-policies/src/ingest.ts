import { ExternalConnectors } from '@microsoft/microsoft-graph-types';
import { getClient } from './graphClient';
import fs from 'fs';
import matter, { GrayMatterFile } from 'gray-matter';
import path from 'path';
import removeMd from 'remove-markdown';
import { Config } from './models/Config';

const client = getClient();

/**
 * Extracts the content from the files in the content directory.
 * @param config - The configuration object.
 * @returns An array of GrayMatterFile objects.
 */
function extractContent(config: Config): GrayMatterFile<string>[] {
  let content = [];
  let contentFiles = fs.readdirSync('./content');

  contentFiles.forEach(f => {
    if (!f.endsWith('.md')) {
      return;
    }

    const fileContents = fs.readFileSync(path.resolve('./content', f), 'utf-8');
    const doc = matter(fileContents);
    doc.content = removeMd(doc.content.replace(/<[^>]+>/g, ' '));
    doc.data.url = new URL(doc.data.policyNumber, config.connector.baseUrl).href;
    content.push(doc);
  });

  return content;
}

/**
 * Transforms the content into a format that can be ingested by the Graph API.
 * @param content
 * @returns An array of objects that can be ingested by the Graph API.
 */
function transformContent(content: GrayMatterFile<string>[]): ExternalConnectors.ExternalItem[] {
  return content.map(doc => {
    return {
      id: doc.data.policyNumber,
      properties: {
        lastModified: new Date(doc.data.lastModified).toISOString().slice(0, -5) + 'Z',
        title: doc.data.title,
        abstract: doc.data.abstract,
        author: doc.data.author,
        policyNumber: doc.data.policyNumber,
        url: doc.data.url
      },
      content: {
        value: doc.content,
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
  const files = await extractContent(config);
  for (const doc of transformContent(files)) {
    await loadContent(config, doc);
  }
}

import { ExternalConnectors } from '@microsoft/microsoft-graph-types';
import { config } from './config';
import { getClient } from './graphClient';
import fs from 'fs';
import matter, { GrayMatterFile } from 'gray-matter';
import path from 'path';
import removeMd from 'remove-markdown';

const client = getClient();

/**
 * Extracts the content from the files in the content directory.
 * @returns An array of GrayMatterFile objects.
 */
function extractContent(): GrayMatterFile<string>[] {
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
 * @param content
 * @returns A promise that resolves when the content has been loaded.
 */
async function loadContent(doc: ExternalConnectors.ExternalItem): Promise<void> {
  const { id } = config.connector;

  try {
    console.log(`Loading ${doc.id}...`);
    await client
      .api(`/external/connections/${id}/items/${doc.id}`)
      .header('content-type', 'application/json')
      .put(doc);
  } catch (e) {
    console.error(`Failed to load ${doc.id}: ${e.message}`);
    if (e.body) {
      console.error(`${JSON.parse(e.body, null)?.innerError?.message}`);
    }
    return;
  }
}

/**
 * Ensures that the content is ingested into the Graph API.
 */
export async function ingestContent() {
  const files = await extractContent();
  for (const doc of transformContent(files)) {
    await loadContent(doc);
  }
}
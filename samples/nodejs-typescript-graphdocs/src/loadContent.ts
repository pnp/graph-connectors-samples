import { GraphError } from '@microsoft/microsoft-graph-client';
import { ExternalConnectors } from '@microsoft/microsoft-graph-types';
import fs from 'fs';
import matter, { GrayMatterFile } from 'gray-matter';
import path from 'path';
import removeMd from 'remove-markdown';
import { config } from './config.js';
import { client } from './graphClient.js';

const contentDir = 'content';

interface Document extends GrayMatterFile<string> {
  content: string;
  relativePath: string;
}

function getDocId(relativePath: string): string {
  const id = relativePath.replace(path.sep, '__').replace('.md', '');
  return id;
}

function extract(): Document[] {
  const content: Document[] = [];
  const contentFiles = fs.readdirSync(contentDir, { recursive: true });

  contentFiles.forEach(file => {
    if (!file.toString().endsWith('.md')) {
      return;
    }

    const fileContents = fs.readFileSync(path.join(contentDir, file.toString()), 'utf-8');
    const doc = matter(fileContents) as Document;

    doc.content = removeMd(doc.content.replace(/<[^>]+>/g, ' '));
    doc.relativePath = file.toString();

    content.push(doc);
  });

  return content;
}

function transform(documents: Document[]): ExternalConnectors.ExternalItem[] {
  const baseUrl = 'https://learn.microsoft.com/graph/';

  return documents.map(doc => {
    const docId = getDocId(doc.relativePath ?? '');
    return {
      id: docId,
      properties: {
        title: doc.data.title ?? '',
        description: doc.data.description ?? '',
        url: new URL(doc.relativePath.replace('.md', ''), baseUrl).toString(),
        iconUrl: 'https://raw.githubusercontent.com/waldekmastykarz/img/main/microsoft-graph.png'
      },
      content: {
        value: doc.content ?? '',
        type: 'text'
      },
      acl: [
        {
          accessType: 'grant',
          type: 'everyone',
          value: 'everyone'
        }
      ]
    } as ExternalConnectors.ExternalItem
  });
}

async function load(externalItems: ExternalConnectors.ExternalItem[]) {
  const { id } = config.connection;
  for (const doc of externalItems) {
    try {
      console.log(`Loading ${doc.id}...`);
      await client
        .api(`/external/connections/${id}/items/${doc.id}`)
        .header('content-type', 'application/json')
        .put(doc);
      console.log('  DONE');
    }
    catch (e) {
      const graphError = e as GraphError;
      console.error(`Failed to load ${doc.id}: ${graphError.message}`);
      if (graphError.body) {
        console.error(`${JSON.parse(graphError.body)?.innerError?.message}`);
      }
      return;
    }
  }
}

export async function loadContent() {
  const content = extract();
  const transformed = transform(content);
  await load(transformed);
}

loadContent();
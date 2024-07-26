import { config } from './Config';
import { client } from './GraphClient';
import fs from 'fs';
import matter, { GrayMatterFile } from 'gray-matter';
import path from 'path';

function extractContent(): GrayMatterFile<string>[] {
  var content = [];
  var contentFiles = fs.readdirSync('../content');

  contentFiles.forEach(f => {
    if (!f.endsWith('.md')) {
      return;
    }

    const fileContents = fs.readFileSync(path.resolve('../content', f), 'utf-8');
    const doc = matter(fileContents);
    doc.data.url = new URL(doc.data.policyNumber, config.connector.baseUrl).href;
    content.push(doc);
  });

  return content;
}

function transformContent(content) {
  return content.map(doc => {
    return {
      id: doc.data.policyNumber,
      properties: {
        lastModified: new Date(doc.data.lastModified).toISOString(),
        title: doc.data.title,
        abstract: doc.data.abstract,
        author: doc.data.author,
        policyNumber: doc.data.policyNumber,
        url: doc.data.url,
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
    }
  });
}

async function loadContent(content) {
  const { id } = config.connector;

  for (const doc of transformContent(content)) {
    try {
      console.log(`Loading ${doc.id}...`);
      await client
        .api(`/external/connections/${id}/items/${doc.id}`)
        .header('content-type', 'application/json')
        .put(doc);
    }
    catch (e) {
      console.error(`Failed to load ${doc.id}: ${e.message}`);
      if (e.body) {
        console.error(`${JSON.parse(e.body, null)?.innerError?.message}`);
      }
      return;
    }
  }
}

export async function ensureIngestion() {
  const files = await extractContent();
  await loadContent(files);
}
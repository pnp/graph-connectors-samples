import fs from 'fs';
import matter from 'gray-matter';
import path from 'path';
import removeMd from 'remove-markdown';
import url from 'url';
import { config } from './config.js';
import { client } from './graphClient.js';

const __dirname = url.fileURLToPath(new URL('.', import.meta.url));
const contentDir = path.join(__dirname, 'content');
const baseUrl = 'https://blog.mastykarz.nl';

function extract() {
  var content = [];
  var contentFiles = fs.readdirSync(contentDir);

  contentFiles.forEach(f => {
    if (!f.endsWith('.markdown') && !f.endsWith('.md')) {
      return;
    }

    const fileContents = fs.readFileSync(path.resolve(contentDir, f), 'utf-8');
    const doc = matter(fileContents);

    doc.content = removeMd(doc.content.replace(/<[^>]+>/g, ' '));
    doc.url = url.resolve(baseUrl, doc.data.slug);
    doc.data.image = url.resolve(baseUrl, doc.data.image);

    content.push(doc);
  });

  return content;
}

function transform(content) {
  return content.map(doc => {
    // Date must be in the ISO 8601 format
    const docDate = new Date(doc.data.date).toISOString();
    return {
      id: doc.data.slug,
      properties: {
        title: doc.data.title,
        excerpt: doc.data.excerpt,
        imageUrl: doc.data.image,
        url: doc.url,
        date: docDate,
        'tags@odata.type': 'Collection(String)',
        tags: doc.data.tags
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
      ],
      // If you'd like to add the created activity,
      // in the config.js file, add the userId property mapped to your user's ID
      // and then uncomment the following lines:
      //
      // activities: [{
      //   '@odata.type': '#microsoft.graph.externalConnectors.externalActivity',
      //   type: 'created',
      //   startDateTime: docDate,
      //   performedBy: {
      //     type: 'user',
      //     id: config.userId
      //   }
      // }]
    }
  });
}

async function load(content) {
  const { id } = config.connector;
  for (const doc of content) {
    try {
      console.log(`Loading ${doc.id}...`);
      await client
        .api(`/external/connections/${id}/items/${doc.id}`)
        .header('content-type', 'application/json')
        .put(doc);
      console.log('  DONE');
    }
    catch (e) {
      console.error(`Failed to load ${doc.id}: ${e.message}`);
      if (e.body) {
        console.error(`${JSON.parse(e.body, null, 2)?.innerError?.message}`);
      }
      return;
    }
  }
}

async function main() {
  const content = extract();
  const transformed = transform(content);
  await load(transformed);
}

main();
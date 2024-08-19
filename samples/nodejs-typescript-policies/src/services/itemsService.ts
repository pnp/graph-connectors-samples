import fs from 'fs';
import matter, { GrayMatterFile } from 'gray-matter';
import path from 'path';
import removeMd from 'remove-markdown';
import { Config } from '../models/Config';
import { Item } from '../models/Item';

/**
 * Extracts the content from the files in the content directory.
 * @param config - The configuration object.
 * @returns An array of GrayMatterFile objects.
 */
function extractDocuments(config: Config): GrayMatterFile<string>[] {
  let content = [];
  let contentFiles = fs.readdirSync('./content');

  contentFiles.forEach(f => {
    if (!f.endsWith('.md')) {
      return;
    }

    const fileContents = fs.readFileSync(path.resolve('./content', f), 'utf-8');
    const doc = matter(fileContents);
    doc.content = removeMd(doc.content.replace(/<[^>]+>/g, ' '));
    doc.data.url = new URL(doc.data.id, config.connector.baseUrl).href;
    content.push(doc);
  });

  return content;
}

export function getAllItems(config: Config): Item[] {
  const content = extractDocuments(config);

  return content.map(doc => {
    return {
      id: doc.data.id,
      lastModified: new Date(doc.data.lastModified),
      title: doc.data.title,
      abstract: doc.data.abstract,
      author: doc.data.author,
      content: doc.content,
      url: doc.data.url
    } as Item;
  });
}

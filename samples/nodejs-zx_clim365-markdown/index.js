#!/usr/bin/env zx
$.verbose = false;

import fs from 'fs';
import matter from 'gray-matter';
import path from 'path';
import removeMd from 'remove-markdown';
import url from 'url';
import { config } from './config.js';

const contentDir = path.join(__dirname, 'content');
const baseUrl = 'https://blog.mastykarz.nl';

console.log('Creating connection...');
await $`m365 external connection add --id ${config.id} --name ${config.name} --description ${config.description}`;

console.log('Configuring activity settings...');
for (const resolver of config.activitySettings.urlToItemResolvers) {
  console.log(`Adding URL to item resolver for ${resolver.urlMatchInfo.baseUrls.join(', ')} ${resolver.urlMatchInfo.urlPattern}...`);
  await $`m365 external connection urltoitemresolver add --externalConnectionId "${config.id}" --baseUrls "${resolver.urlMatchInfo.baseUrls.join(',')}" --urlPattern "${resolver.urlMatchInfo.urlPattern}" --itemId "${resolver.itemId}" --priority ${resolver.priority}`;
}

console.log('Creating schema...');
await $`m365 external connection schema add --externalConnectionId ${config.id} --schema ${JSON.stringify(config.schema)} --wait`

console.log('Ingesting content...');
const contentFiles = fs.readdirSync(contentDir);

for (const f of contentFiles) {
  if (!f.endsWith('.markdown') && !f.endsWith('.md')) {
    continue;
  }

  const fileContents = fs.readFileSync(path.resolve(contentDir, f), 'utf-8');
  const doc = matter(fileContents);

  doc.content = removeMd(doc.content.replace(/<[^>]+>/g, ' '));
  doc.url = url.resolve(baseUrl, doc.data.slug);
  doc.data.image = url.resolve(baseUrl, doc.data.image);
  const docDate = new Date(doc.data.date).toISOString();

  try {
    console.log(`Ingesting ${f}...`);
    await $`m365 external item add --id "${doc.data.slug}" --externalConnectionId "${config.id}" --content ${doc.content} --title ${doc.data.title} --excerpt ${doc.data.excerpt} --imageUrl "${doc.data.image}" --url "${doc.url}" --date "${docDate}" --tags@odata.type "Collection(String)" --tags ${doc.data.tags.join(';#')} --acls "grant,everyone,everyone"`
  }
  catch (e) {
    process.exit(1);
    console.error(`Failed to ingest ${f}: ${e}`);
  }
};
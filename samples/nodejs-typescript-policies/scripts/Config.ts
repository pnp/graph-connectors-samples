import dotenv from 'dotenv'; 
import minimist from 'minimist';
import fs from 'fs';
const argv = minimist(process.argv.slice(2));

dotenv.config({
    path: [`./env/.env.${argv.env}`, `./env/.env.${argv.env}.user`]
});

const schemaFile = fs.readFileSync('./schema.json', 'utf8');
const schema = JSON.parse(schemaFile);

export const config = {
    tenantId: process.env.AAD_APP_TENANT_ID,
    clientId: process.env.AAD_APP_CLIENT_ID,
    clientSecret: argv.secret,
    connector: {
      id: process.env.CONNECTOR_ID,
      name: process.env.CONNECTOR_NAME,
      description: process.env.CONNECTOR_DESCRIPTION,
      schema: schema,
      baseUrl: process.env.CONNECTOR_BASE_URL
    }
  };
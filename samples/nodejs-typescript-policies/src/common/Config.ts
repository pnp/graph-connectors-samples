import minimist from 'minimist';
import schema from '../schema.json';

const argv = minimist(process.argv.slice(2));

export const config = {
  debug: argv.debug ?? false,
  //tenantId: process.env.AAD_APP_TENANT_ID,
  clientId: process.env.AZURE_CLIENT_ID,
  //clientSecret: argv.secret,
  connector: {
    id: `${process.env.CONNECTOR_ID}${process.env.TEAMSFX_ENV}`,
    name: process.env.CONNECTOR_NAME,
    description: process.env.CONNECTOR_DESCRIPTION,
    schema: schema,
    baseUrl: process.env.CONNECTOR_BASE_URL
  }
}
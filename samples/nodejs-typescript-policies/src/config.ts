import schema from './references/schema.json';
import template from './references/template.json';

/**
 * Builds the configuration object based on environment variables.
 */
export const config = {
  clientId: process.env.AZURE_CLIENT_ID,
  connector: {
    id: `${process.env.CONNECTOR_ID}${process.env.TEAMSFX_ENV}`,
    name: process.env.CONNECTOR_NAME,
    description: process.env.CONNECTOR_DESCRIPTION,
    schema: schema,
    baseUrl: process.env.CONNECTOR_BASE_URL,
    template: template
  }
};

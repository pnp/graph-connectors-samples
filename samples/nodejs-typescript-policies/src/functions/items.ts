import { app, HttpRequest, HttpResponseInit, InvocationContext, Timer } from '@azure/functions';
import { ensureConnection, setSearchSettings } from '../connection';
import { ensureSchema } from '../schema';
import { ingestContent } from '../ingest';
import { initConfig } from '../config';
import { getAllItems } from '../services/itemsService';

/**
 * The deployItems function is responsible for ensuring the connection, schema, and search settings are in place.
 * It also starts the ingestion of the content.
 * @param {Timer} timer - The timer object
 * @param {InvocationContext} context - The context object of the current invocation
 * @returns {Promise<void>}
 */
export async function deployItems(timer: Timer, context: InvocationContext): Promise<void> {
  // Initializes the configuration for the current invocation
  const config = initConfig(context);

  // Creates the connection
  let connectionResult = await ensureConnection(config);
  if (connectionResult) {
    // Creates the schema
    await ensureSchema(config);

    // Updates the search settings with the result template
    await setSearchSettings(config);

    // Starts the ingestion of the content
    await ingestContent(config);
  }
}

/**
 * Gets all items from the system
 * @param request Gets
 * @param context
 * @returns
 */
export async function getItems(request: HttpRequest, context: InvocationContext): Promise<HttpResponseInit> {
  // Initializes the configuration for the current invocation
  const config = initConfig(context);

  return { jsonBody: getAllItems(config) };
}

app.http('items', {
  methods: ['GET'],
  authLevel: 'anonymous',
  handler: getItems
});

app.timer('deployItems', {
  schedule: '0 0 0 30 2 *',
  runOnStartup: true,
  handler: deployItems
});

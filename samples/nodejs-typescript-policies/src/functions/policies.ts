import { app } from '@azure/functions';
import { ensureConnection, setSearchSettings } from '../connection';
import { ensureSchema } from '../schema';
import { ingestContent } from '../ingest';

/**
 * @returns {Promise<void>}
 */
export async function policies(): Promise<void> {
  // Creates the connection
  let connectionResult = await ensureConnection();
  if (connectionResult) {
    // Creates the schema
    await ensureSchema();

    // Updates the search settings with the result template
    await setSearchSettings();

    // Starts the ingestion of the content
    await ingestContent();
  }
}

app.timer('policies', {
  schedule: '0 0 0 30 2 *',
  runOnStartup: true,
  handler: policies
});

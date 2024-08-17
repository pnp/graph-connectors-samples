import { ensureConnection, setSearchSettings } from './Connection';
import { ensureSchema } from './Schema';
import { ensureIngestion } from './Ingest';

/**
 * Orchestrator for the full deployment of the Graph connector 
 * and ingestion of the content.
 */
export async function DeployConnections() {
  // Creates the connection
  var connectionResult = await ensureConnection();
  if(connectionResult) {

    // Creates the schema
    await ensureSchema();

    // Updates the search settings with the result template
    await setSearchSettings();

    // Starts the ingestion of the content
    await ensureIngestion();
  }
}
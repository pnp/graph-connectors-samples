import { ensureConnection } from './EnsureConnection';
import { ensureSchema } from './EnsureSchema';
import { ensureIngestion } from './IngestContent';

/**
 * Orchestrator for the full deployment of the Graph connector 
 * and ingestion of the content.
 */
export async function DeployConnections() {
  var connectionResult = await ensureConnection();
  if(connectionResult) {
    await ensureSchema();
    await ensureIngestion();
  }
}
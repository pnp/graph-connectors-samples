import { ensureConnection } from './EnsureConnection';
import { ensureSchema } from './EnsureSchema';
import { ensureIngestion } from './IngestContent';

export async function DeployConnections() {
  var connectionResult = await ensureConnection();
  if(connectionResult) {
    await ensureSchema();
    await ensureIngestion();
  }
}
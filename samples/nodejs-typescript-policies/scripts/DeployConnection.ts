import { ensureConnection } from './EnsureConnection';
import { ensureSchema } from './EnsureSchema';
import { ensureIngestion } from './IngestContent';

async function main() {
  var connectionResult = await ensureConnection();
  if(connectionResult) {
    await ensureSchema();
    await ensureIngestion();
  }
}

main();
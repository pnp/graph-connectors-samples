import { createConnection, createSchema } from './manageConnection.js';

async function main() {
  try {
    await createConnection();
    await createSchema();
  }
  catch (e) {
    console.error(e);
  }
}

main();
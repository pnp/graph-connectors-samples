import bodyParser from 'body-parser';
import express from 'express';
import { appInfo } from './env.js';
import { createConnection, createSchema, deleteConnection } from './manageConnection.js';
import { loadContent } from './manageContent.js';
import { validateToken } from './validateToken.js';

async function processNotification(notification) {
  try {
    await validateToken(notification.token, notification.tenantId, notification.clientId);

    switch (notification.state) {
      case 'enabled':
        await createConnection(notification.ticket);
        await createSchema();
        await loadContent();
        break;
      case 'disabled':
        await deleteConnection(notification.ticket);
        break;
      default:
        throw `Unknown state: ${notification.state}`;
    }
  }
  catch (err) {
    console.error(err);
    return;
  }
}

const app = express();
app.use(bodyParser.json());

app.post('/api/notification', async (req, res) => {
  res.status(202).send();

  const token = req.body.validationTokens[0];
  const tenantId = req.body.value[0].tenantId;
  const clientId = appInfo.appId;
  const ticket = req.body.value[0].resourceData.connectorsTicket;
  const state = req.body.value[0].resourceData.state;

  processNotification({ token, tenantId, clientId, ticket, state });
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Listening for Teams Admin Center notification on http://localhost:${PORT}/api/notification`));
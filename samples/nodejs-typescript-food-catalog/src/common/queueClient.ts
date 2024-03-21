import { QueueServiceClient } from '@azure/storage-queue';
import { ConnectionMessage } from './ConnectionMessage';
import { ContentMessage, CrawlType } from './ContentMessage';
import { config } from './config';

export async function getQueueClient(queueName: string) {
  const { storageAccountConnectionString } = config;
  const queueServiceClient = QueueServiceClient.fromConnectionString(storageAccountConnectionString);
  await queueServiceClient.createQueue(queueName);
  return queueServiceClient.getQueueClient(queueName);
}

export async function enqueueCheckStatus(location: string) {
  const message: ConnectionMessage = {
    action: 'status',
    location
  }
  const queueClient = await getQueueClient('queue-connection');
  // wait 60s before polling again for status
  await queueClient.sendMessage(btoa(JSON.stringify(message)), { visibilityTimeout: config.graphSchemaStatusInterval });
}

export async function startCrawl(crawlType: CrawlType) {
  const queueClient = await getQueueClient('queue-content');
  const message: ContentMessage = {
    action: 'crawl',
    crawlType: crawlType
  }
  await queueClient.sendMessage(btoa(JSON.stringify(message)));
}

export async function enqueueItemUpdate(itemId: string) {
  const queueClient = await getQueueClient('queue-content');
  const message: ContentMessage = {
    action: 'item',
    itemAction: 'update',
    itemId
  }
  await queueClient.sendMessage(btoa(JSON.stringify(message)));
}

export async function enqueueItemDeletion(itemId: string) {
  const queueClient = await getQueueClient('queue-content');
  const message: ContentMessage = {
    action: 'item',
    itemAction: 'delete',
    itemId
  }
  await queueClient.sendMessage(btoa(JSON.stringify(message)));
}
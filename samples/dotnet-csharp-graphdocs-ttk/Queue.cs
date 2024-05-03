using Azure.Storage.Queues;
using GraphDocsConnector.Messages;
using System.Text.Json;

namespace GraphDocsConnector
{
    internal static class Queue
    {
        public static void EnqueueCheckStatus(QueueClient queueClient, string location)
        {
            var message = new ConnectionMessage
            {
                Action = ConnectionMessageAction.Status,
                Location = location
            };

            queueClient.CreateIfNotExists();
            queueClient.SendMessage(
                JsonSerializer.Serialize(message),
                TimeSpan.FromSeconds(int.Parse(Environment.GetEnvironmentVariable("GRAPH_SCHEMA_STATUS_INTERVAL") ?? "60"))
            );
        }

        public static void StartCrawl(QueueClient queueClient, CrawlType crawlType)
        {
            var message = new ContentMessage
            {
                Action = ContentAction.Crawl,
                CrawlType = crawlType
            };
            queueClient.CreateIfNotExists();
            queueClient.SendMessage(JsonSerializer.Serialize(message));
        }

        public static void EnqueueItemUpdate(QueueClient queueClient, string itemId)
        {
            var message = new ContentMessage
            {
                Action = ContentAction.Item,
                ItemAction = ItemAction.Update,
                ItemId = itemId
            };
            queueClient.CreateIfNotExists();
            queueClient.SendMessage(JsonSerializer.Serialize(message));
        }

        internal static void EnqueueItemDeletion(QueueClient queueClient, string itemId)
        {
            var message = new ContentMessage
            {
                Action = ContentAction.Item,
                ItemAction = ItemAction.Delete,
                ItemId = itemId
            };
            queueClient.CreateIfNotExists();
            queueClient.SendMessage(JsonSerializer.Serialize(message));
        }
    }
}

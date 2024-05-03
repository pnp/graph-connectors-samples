using Azure.Storage.Queues;
using GraphDocsConnector.Messages;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GraphDocsConnector.Functions
{
    public class TimerRemoveDeleted
    {
        private readonly ILogger _logger;
        private readonly QueueClient _queueContentClient;

        public TimerRemoveDeleted(QueueServiceClient queueClient, ILoggerFactory loggerFactory)
        {
            _queueContentClient = queueClient.GetQueueClient("queue-content");
            _logger = loggerFactory.CreateLogger<TimerRemoveDeleted>();
        }

        [Function("TimerRemoveDeleted")]
        public void Run([TimerTrigger("10 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation("Enqueueing request for removing deleted items...");
            _queueContentClient.CreateIfNotExistsAsync();
            Queue.StartCrawl(_queueContentClient, CrawlType.RemoveDeleted);
        }
    }
}

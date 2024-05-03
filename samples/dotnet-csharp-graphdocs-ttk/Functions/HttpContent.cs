using Azure.Storage.Queues;
using GraphDocsConnector.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GraphDocsConnector.Functions
{
    public class CrawlRequest
    {
        public CrawlType? CrawlType { get; set; }
    }

    public class HttpContent
    {
        private readonly ILogger<HttpContent> _logger;
        private readonly QueueClient _queueContentClient;

        public HttpContent(QueueClient queueClient, ILogger<HttpContent> logger)
        {
            _queueContentClient = queueClient;
            _logger = logger;
        }

        [Function("crawl")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, [FromBody] CrawlRequest crawlRequest)
        {
            if (crawlRequest.CrawlType is null)
            {
                return new BadRequestResult();
            }

            _logger.LogInformation($"Enqueuing crawl request for {crawlRequest.CrawlType}...");
            Queue.StartCrawl(_queueContentClient, crawlRequest.CrawlType.Value);

            return new AcceptedResult();
        }
    }
}

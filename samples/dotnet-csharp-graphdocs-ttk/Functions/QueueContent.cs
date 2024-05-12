using System.Diagnostics;
using System.Text.Json;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using GraphDocsConnector.Messages;
using Markdig;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models.ExternalConnectors;
using YamlDotNet.Serialization;

namespace GraphDocsConnector.Functions
{
    class DocsArticle : IMarkdown
    {
        [YamlMember(Alias = "title")]
        public string? Title { get; set; }
        [YamlMember(Alias = "description")]
        public string? Description { get; set; }
        public string? Markdown { get; set; }
        public string? Content { get; set; }
        public string? RelativePath { get; set; }
    }

    public class QueueContent
    {
        private readonly ILogger<QueueContent> _logger;
        private readonly GraphServiceClient _graphClient;
        private readonly HttpClient _documentsClient;
        private readonly QueueClient _queueContentClient;
        private readonly TableClient _tableExternalItemsClient;
        private readonly TableClient _tableStateClient;

        public QueueContent(GraphServiceClient graphServiceClient, DocumentsServiceClient documentServiceClient, QueueServiceClient queueClient, TableServiceClient tableClient, ILogger<QueueContent> logger)
        {
            _graphClient = graphServiceClient;
            _documentsClient = documentServiceClient.Client;
            _queueContentClient = queueClient.GetQueueClient("queue-content");
            _tableExternalItemsClient = tableClient.GetTableClient("externalitems");
            _tableStateClient = tableClient.GetTableClient("state");
            _logger = logger;
        }

        [Function(nameof(QueueContent))]
        public async Task Run([QueueTrigger("queue-content", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            var contentMessage = JsonSerializer.Deserialize<ContentMessage>(message.MessageText);
            if (contentMessage is null)
            {
                _logger.LogError($"Couldn't deserialize message: {message.MessageText}");
                return;
            }

            Debug.Assert(contentMessage.Action is not null);

            switch (contentMessage.Action)
            {
                case ContentAction.Crawl:
                    await Crawl(contentMessage.CrawlType);
                    break;
                case ContentAction.Item:
                    await ProcessItem(contentMessage.ItemId, contentMessage.ItemAction);
                    break;
            }
        }

        private async Task ProcessItem(string? itemId, ItemAction? itemAction)
        {
            if (itemAction is null)
            {
                _logger.LogError("itemAction is null");
                return;
            }

            if (itemId is null)
            {
                _logger.LogError("itemId is null");
                return;
            }

            switch (itemAction)
            {
                case ItemAction.Update:
                    await UpdateItem(itemId);
                    break;
                case ItemAction.Delete:
                    await DeleteItem(itemId);
                    break;
            }
        }

        private async Task DeleteItem(string itemId)
        {
            _logger.LogInformation($"Deleting item {itemId}...");
            await _graphClient.External
                .Connections[ConnectionConfiguration.ExternalConnection.Id]
                .Items[itemId]
                .DeleteAsync();
        }

        private async Task UpdateItem(string itemId)
        {
            var url = $"{Environment.GetEnvironmentVariable("DocumentsApi:Url")}/documents/{itemId}";

            _logger.LogInformation($"Retrieving item from {url}...");

            FileInfo? file = null;

            try
            {
                var res = await _documentsClient.GetStringAsync(url);
                file = JsonSerializer.Deserialize<FileInfo>(res, Utils.JsonSerializerOptions);
                if (file is null)
                {
                    _logger.LogWarning("Received null response");
                    return;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error has occurred while retrieving items");
                return;
            }

            _logger.LogDebug($"Retrieved item from {url}");
            _logger.LogDebug(JsonSerializer.Serialize(file));

            if (file.Id is null)
            {
                _logger.LogWarning("File ID is null");
                return;
            }

            if (file.Contents is null)
            {
                _logger.LogWarning("File has no content");
                return;
            }

            var baseUrl = new Uri("https://learn.microsoft.com/graph/");

            var doc = file.Contents.GetContents<DocsArticle>();
            doc.Content = Markdown.ToHtml(doc.Markdown ?? "");

            var externalItem = new ExternalItem
            {
                Id = file.Id,
                Properties = new()
                {
                    AdditionalData = new Dictionary<string, object>
                    {
                        { "title", doc.Title ?? "" },
                        { "description", doc.Description ?? "" },
                        { "url", new Uri(baseUrl, file.Id.Replace("__", "/").Replace(".md", "")).ToString() }
                    }
                },
                Content = new()
                {
                    Value = doc.Content ?? "",
                    Type = ExternalItemContentType.Html
                },
                Acl = new()
                {
                    new()
                    {
                        Type = AclType.Everyone,
                        Value = "everyone",
                        AccessType = AccessType.Grant
                    }
                }
            };

            _logger.LogInformation($"Loading item {externalItem.Id}...");
            await _graphClient.External
                .Connections[ConnectionConfiguration.ExternalConnection.Id]
                .Items[externalItem.Id]
                .PutAsync(externalItem);

            _logger.LogInformation($"Adding item {externalItem.Id} to table storage...");
            Table.AddItem(_tableExternalItemsClient, externalItem.Id);

            _logger.LogInformation($"Tracking last modified date {file.LastModified}...");
            Table.RecordLastModified(_tableStateClient, file.LastModified, _logger);
        }

        private async Task Crawl(CrawlType? crawlType)
        {
            if (crawlType is null)
            {
                _logger.LogError("crawlType is null");
                return;
            }

            switch (crawlType)
            {
                case CrawlType.Full:
                case CrawlType.Incremental:
                    await CrawlFullOrIncremental(crawlType);
                    break;
                case CrawlType.RemoveDeleted:
                    await RemoveDeleted();
                    break;
            }
        }

        private async Task RemoveDeleted()
        {
            var url = $"{Environment.GetEnvironmentVariable("DocumentsApi:Url")}/documents";

            try
            {
                var res = await _documentsClient.GetStringAsync(url);
                var files = JsonSerializer.Deserialize<FileInfo[]>(res, Utils.JsonSerializerOptions);
                if (files is null)
                {
                    _logger.LogWarning("Received null response");
                    return;
                }

                _logger.LogInformation($"Retrieved {files.Length} items from {url}");

                var ingestedItemIds = Table.GetItemIds(_tableExternalItemsClient);

                foreach (var ingestedItemId in ingestedItemIds)
                {
                    if (files.Any(f => f.Id == ingestedItemId))
                    {
                        _logger.LogInformation($"Item {ingestedItemId} still exists, skipping...");
                    }
                    else
                    {
                        _logger.LogInformation($"Item {ingestedItemId} no longer exists, deleting...");
                        Queue.EnqueueItemDeletion(_queueContentClient, ingestedItemId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error has occurred while retrieving items");
            }
        }

        private async Task CrawlFullOrIncremental(CrawlType? crawlType)
        {
            var url = $"{Environment.GetEnvironmentVariable("DocumentsApi:Url")}/documents";

            if (crawlType == CrawlType.Incremental)
            {
                var lastModified = Table.GetLastModified(_tableStateClient);
                url += $"?$filter=lastModified gt {lastModified}";
            }

            _logger.LogInformation($"Retrieving items from {url}...");

            try
            {
                var res = await _documentsClient.GetStringAsync(url);
                var files = JsonSerializer.Deserialize<FileInfo[]>(res, Utils.JsonSerializerOptions);
                if (files is null)
                {
                    _logger.LogWarning("Received null response");
                    return;
                }

                _logger.LogInformation($"Retrieved {files.Length} items from {url}");
                foreach (var file in files)
                {
                    if (file.Id is null)
                    {
                        _logger.LogWarning($"ID is null. Skipping...");
                        continue;
                    }

                    _logger.LogInformation($"Enqueueing item update for {file.Id}...");
                    Queue.EnqueueItemUpdate(_queueContentClient, file.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error has occurred while retrieving items");
            }
        }
    }
}

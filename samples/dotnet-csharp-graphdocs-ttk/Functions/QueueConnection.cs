using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using GraphDocsConnector.Messages;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models.ExternalConnectors;

namespace GraphDocsConnector.Functions
{
    public class QueueConnection
    {
        private readonly ILogger<QueueConnection> _logger;
        private readonly GraphServiceClient _graphClient;
        private readonly QueueClient _queueConnectionClient;
        private readonly QueueClient _queueContentClient;

        public QueueConnection(QueueServiceClient queueClient, GraphServiceClient graphClient, ILogger<QueueConnection> logger)
        {
            _queueConnectionClient = queueClient.GetQueueClient("queue-connection");
            _queueContentClient = queueClient.GetQueueClient("queue-content");
            _graphClient = graphClient;
            _logger = logger;
        }

        [Function(nameof(QueueConnection))]
        public async Task Run([QueueTrigger("queue-connection", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            var connectionMessage = JsonSerializer.Deserialize<ConnectionMessage>(message.MessageText);
            if (connectionMessage is null)
            {
                _logger.LogError($"Couldn't deserialize message: {message.MessageText}");
                return;
            }

            switch (connectionMessage.Action)
            {
                case ConnectionMessageAction.Create:
                    _logger.LogInformation("Creating connection...");
                    await CreateConnection(connectionMessage);
                    _logger.LogInformation("Connection created");
                    _logger.LogInformation("Submitting schema for provisioning...");
                    await CreateSchema();
                    _logger.LogInformation("Schema submitted");
                    break;
                case ConnectionMessageAction.Delete:
                    _logger.LogInformation("Deleting connection...");
                    await DeleteConnection();
                    _logger.LogInformation("Connection deleted");
                    break;
                case ConnectionMessageAction.Status:
                    _logger.LogInformation("Checking schema status...");
                    await CheckSchemaStatus(connectionMessage.Location!);
                    _logger.LogInformation("Schema status checked");
                    break;
            }
        }

        private async Task CheckSchemaStatus(string location)
        {
            var res = await _graphClient.External
                .Connections[ConnectionConfiguration.ExternalConnection.Id]
                // intentionally empty because we're getting the operation ID
                // from the location using the WithUrl construct
                .Operations[""]
                .WithUrl(location).GetAsync();

            if (res is null)
            {
                _logger.LogError($"Operation status for {location} is null");
                return;
            }

            _logger.LogInformation($"Schema provisioning status: {res.Status}");

            switch (res.Status)
            {
                case ConnectionOperationStatus.Inprogress:
                    Queue.EnqueueCheckStatus(_queueConnectionClient, location);
                    break;
                case ConnectionOperationStatus.Completed:
                    Queue.StartCrawl(_queueContentClient, CrawlType.Full);
                    break;
                default:
                    _logger.LogWarning($"Unsupported status");
                    break;
            }
        }

        private async Task CreateConnection(ConnectionMessage connectionMessage)
        {
            var externalConnection = ConnectionConfiguration.ExternalConnection;
            externalConnection.ConnectorId = connectionMessage.ConnectorId;
            
            await _graphClient.External.Connections
                .PostAsync(externalConnection, request =>
                {
                    request.Headers.Add("GraphConnectors-Ticket", connectionMessage.ConnectorTicket!);
                });
        }

        private async Task CreateSchema()
        {
            var schemaRequest = _graphClient.External
                .Connections[ConnectionConfiguration.ExternalConnection.Id]
                .Schema
                .ToPatchRequestInformation(ConnectionConfiguration.Schema);
            var httpRequestMessage = await _graphClient.RequestAdapter
                .ConvertToNativeRequestAsync<HttpRequestMessage>(schemaRequest);
            if (httpRequestMessage is null)
            {
                _logger.LogError("httpRequestMessage is null");
                return;
            }

            var httpClient = Utils.GetHttpClient();
            var res = await httpClient.SendAsync(httpRequestMessage);
            var location = res.Headers.GetValues("location")?.FirstOrDefault();
            if (string.IsNullOrEmpty(location))
            {
                _logger.LogError("Schema operation status location is empty");
                return;
            }

            Queue.EnqueueCheckStatus(_queueConnectionClient, location);
        }

        private async Task DeleteConnection()
        {
            await _graphClient.External
                .Connections[ConnectionConfiguration.ExternalConnection.Id]
                .DeleteAsync();
        }
    }
}

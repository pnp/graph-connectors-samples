using Azure.Storage.Queues;
using GraphDocsConnector.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace GraphDocsConnector.Functions
{
    public class HttpNotification
    {
        private readonly ILogger<HttpNotification> _logger;
        private readonly QueueClient _queueClient;
        private readonly IConfiguration _configuration;

        public HttpNotification(QueueServiceClient queueClient, ILogger<HttpNotification> logger, IConfiguration configuration)
        {
            _queueClient = queueClient.GetQueueClient("queue-connection");
            _logger = logger;
            _configuration = configuration;
        }

        [Function("notification")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            var notification = await req.ReadFromJsonAsync<NotificationMessage>();
            await ProcessNotification(notification);
            return new AcceptedResult();
        }

        private async Task ProcessNotification(NotificationMessage? notificationMessage)
        {
            if (notificationMessage is null)
            {
                _logger.LogWarning("Notification message is null");
                return;
            }

            var tenantId = _configuration["Entra:TenantId"];
            var clientId = _configuration["Entra:ClientId"];
            Debug.Assert(tenantId is not null);
            Debug.Assert(clientId is not null);

            var tokenValidator = new TokenValidator(tenantId, clientId);
            if (notificationMessage.ValidationTokens is null ||
                !notificationMessage.ValidationTokens.Any())
            {
                _logger.LogError("No token to validate found");
                return;
            }
            
            if (!tokenValidator.ValidateToken(notificationMessage.ValidationTokens.First()))
            {
                _logger.LogError("Invalid token");
                return;
            }

            var changeDetails = notificationMessage.Value?[0].ResourceData;
            var targetConnectionState = changeDetails?.State;

            if (changeDetails is null || targetConnectionState is null)
            {
                _logger.LogError("Notification payload doesn't contain the necessary information");
                return;
            }

            var message = new ConnectionMessage
            {
                ConnectorId = changeDetails.Id,
                ConnectorTicket = changeDetails.ConnectorsTicket
            };

            switch (targetConnectionState)
            {
                case "enabled":
                    message.Action = ConnectionMessageAction.Create;
                    break;
                case "disabled":
                    message.Action = ConnectionMessageAction.Delete;
                    break;
                default:
                    return;
            }

            await _queueClient.CreateIfNotExistsAsync();
            await _queueClient.SendMessageAsync(JsonSerializer.Serialize(message));
        }
    }
}

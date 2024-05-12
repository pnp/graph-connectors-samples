using System.Text.Json.Serialization;

namespace GraphDocsConnector.Messages
{
    internal class NotificationMessage
    {
        public Notification[]? Value { get; set; }
        public string[]? ValidationTokens { get; set; }
    }

    public class Notification
    {
        public string? ChangeType { get; set; }
        public string? ClientState { get; set; }
        public string? Resource { get; set; }
        public NotificationResourceData? ResourceData { get; set; }
        public string? SubscriptionExpirationDateTime { get; set; }
        public string? SubscriptionId { get; set; }
        public string? TenantId { get; set; }
    }

    public class NotificationResourceData
    {
        public string? ConnectorsTicket { get; set; }
        public string? Id { get; set; }
        [JsonPropertyName("@odata.id")]
        public string? ODataId { get; set; }
        [JsonPropertyName("@odata.type")]
        public string? ODataType { get; set; }
        public string? State { get; set; }
    }
}

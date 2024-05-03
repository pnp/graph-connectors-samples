namespace GraphDocsConnector.Messages
{
    internal class ConnectionMessage
    {
        public ConnectionMessageAction Action { get; set; }
        public string? ConnectorId { get; set; }
        public string? ConnectorTicket { get; set; }
        public string? Location { get; set; }
    }

    internal enum ConnectionMessageAction
    {
        Create,
        Delete,
        Status
    }
}

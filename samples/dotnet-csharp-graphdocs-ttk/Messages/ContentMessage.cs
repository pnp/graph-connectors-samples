namespace GraphDocsConnector.Messages
{
    public enum CrawlType
    {
        Full,
        Incremental,
        RemoveDeleted
    }

    internal enum ItemAction
    {
        Update,
        Delete
    }

    internal enum ContentAction
    {
        Crawl,
        Item
    }

    internal class ContentMessage
    {
        public ContentAction? Action { get; set; }
        public CrawlType? CrawlType { get; set; }
        public ItemAction? ItemAction { get; set; }
        public string? ItemId { get; set; }
    }
}

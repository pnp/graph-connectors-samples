using Azure;
using Azure.Data.Tables;
using Markdig.Extensions.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;

namespace GraphDocsConnector
{
    internal class StateRecord : ITableEntity
    {
        public string Date { get; set; } = "";
        public string PartitionKey { get; set; } = "";
        public string RowKey { get; set; } = "";
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; } = default;
    }

    internal static class Table
    {
        public static void AddItem(TableClient tableClient, string itemId)
        {
            var entity = new TableEntity
            {
                PartitionKey = "documents",
                RowKey = itemId
            };
            tableClient.CreateIfNotExists();
            tableClient.UpsertEntity(entity);
        }

        internal static string[] GetItemIds(TableClient tableClient)
        {
            tableClient.CreateIfNotExists();

            var entities = tableClient.Query<ITableEntity>().ToList();
            var items = new List<string>();

            foreach (var entity in entities)
            {
                items.Add(entity.RowKey);
            }

            return items.ToArray();
        }

        internal static DateTime? GetLastModified(TableClient tableClient)
        {
            tableClient.CreateIfNotExists();

            var lastModifiedFromTable = tableClient.GetEntityIfExists<StateRecord>("state", "lastModified");
            if (!lastModifiedFromTable.HasValue)
            {
                return null;
            }

            return DateTime.Parse(lastModifiedFromTable.Value!.Date);
        }

        internal static void RecordLastModified(TableClient tableClient, DateTime? lastModified, ILogger logger)
        {
            if (lastModified is null)
            {
                return;
            }

            tableClient.CreateIfNotExists();

            var lastModifiedFromTable = tableClient.GetEntityIfExists<StateRecord>("state", "lastModified");
            if (lastModifiedFromTable.HasValue && DateTime.Parse(lastModifiedFromTable.Value!.Date) >= lastModified)
            {
                logger.LogInformation($"Last modified date {lastModifiedFromTable.Value.Date} is newer than {lastModified} or equal");
                return;
            }

            var entity = new StateRecord
            {
                PartitionKey = "state",
                RowKey = "lastModified",
                Date = lastModified.Value.ToString()
            };
            tableClient.UpsertEntity(entity);
        }
    }
}

using System.Text.Json.Serialization;

namespace GraphDocsConnector
{
    public class FileInfo
    {
        public string? Id { get; set; }
        public DateTime? LastModified { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Contents { get; set; }
    }
}

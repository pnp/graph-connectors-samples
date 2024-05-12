using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphDocsConnector
{
    internal class Utils
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = true
        };
        public static JsonSerializerOptions JsonSerializerOptions => jsonSerializerOptions;

        public static HttpClientHandler GetHttpClientHandler()
        {
            return new HttpClientHandler
            {
                Proxy = GetWebProxy()
            };
        }

        public static HttpClient GetHttpClient()
        {
            return new HttpClient(GetHttpClientHandler());
        }

        public static IWebProxy? GetWebProxy()
        {
            var customProxy = Environment.GetEnvironmentVariable("CustomProxy");
            if (string.IsNullOrEmpty(customProxy))
            {
                return null;
            }

            return new WebProxy(customProxy);
        }
    }
}

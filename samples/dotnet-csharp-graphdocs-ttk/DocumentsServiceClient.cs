namespace GraphDocsConnector
{
    public class DocumentsServiceClient
    {
        private HttpClient _client;

        public HttpClient Client { get => _client; }

        public DocumentsServiceClient(HttpClient httpClient)
        {
            _client = httpClient;
        }
    }
}

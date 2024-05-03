using Microsoft.Identity.Client;

namespace GraphDocsConnector
{
    internal class CustomHttpClientFactory : IMsalHttpClientFactory
    {
        private readonly HttpClientHandler _httpClientHandler;

        public CustomHttpClientFactory(HttpClientHandler httpClientHandler)
        {
            _httpClientHandler = httpClientHandler;
        }

        public HttpClient GetHttpClient()
        {
            return new HttpClient(_httpClientHandler);
        }
    }
}

using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace GraphDocsConnector
{
    internal class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly IConfidentialClientApplication _app;
        private readonly string[] _scopes;

        public AuthenticationDelegatingHandler(IConfidentialClientApplication app, string[] scopes)
        {
            _app = app;
            _scopes = scopes;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var tokenResult = await _app.AcquireTokenForClient(_scopes).ExecuteAsync(cancellationToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}

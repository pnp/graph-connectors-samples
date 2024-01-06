using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using O365C.GraphConnector.MicrosoftLearn.Models;
using System;
using System.Threading.Tasks;

namespace O365C.GraphConnector.MicrosoftLearn.Services
{
    public interface IAccessTokenProvider
    {
        Task<string> GetAccessTokenAsync();
    }

    public class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly AzureFunctionSettings _azureFunctionSettings;

        private readonly TokenCredential _tokenCredential;

        public AccessTokenProvider(AzureFunctionSettings azureFunctionSettings)
        {
            _azureFunctionSettings = azureFunctionSettings;
            // Create TokenCredential using client secret
            _tokenCredential = new ClientSecretCredential(_azureFunctionSettings.TenantId, _azureFunctionSettings.ClientId, _azureFunctionSettings.ClientSecret);
        }

        public async Task<string> GetAccessTokenAsync()
        {
            // Use _tokenCredential to get access token
            var tokenRequestContext = new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });
            var accessToken = await _tokenCredential.GetTokenAsync(tokenRequestContext, default);
            return accessToken.Token;
        }
    }
}
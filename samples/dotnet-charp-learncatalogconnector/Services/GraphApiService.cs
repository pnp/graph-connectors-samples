// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Threading.Tasks;
using System;
using Microsoft.Graph.Models.ExternalConnectors;
using O365C.GraphConnector.MicrosoftLearn.Util;
using System.Net.Http;
using System.Linq;
using O365C.GraphConnector.MicrosoftLearn.Models;

namespace O365C.GraphConnector.MicrosoftLearn.Services
{

    public interface IGraphAPIService
    {
        // public GraphServiceClient? GetUserGraphClient(string userAssertion);
        // public GraphServiceClient? GetAppGraphClient();
        Task<UserCollectionResponse> GetUsersAsync();
        Task<ExternalConnection> CreateConnectionAsync();
        Task UpdateConnectionAsync(string connectionId);
        Task<ExternalConnection> GetConnectionAsync(string connectionId);
        Task<ExternalConnectionCollectionResponse> GetExistingConnectionsAsync();
        Task DeleteConnectionAsync(string connectionId);
        Task RegisterSchemaAsync(string connectionId, Schema schema);
        Task<Schema> GetSchemaAsync(string connectionId);
        Task AddOrUpdateItemAsync(string connectionId, ExternalItem item);       

    }
    public class GraphApiService : IGraphAPIService
    {
        private readonly AzureFunctionSettings _configSettings;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private static HttpClient _httpClient;

        private static GraphServiceClient _graphAppClient;

        public GraphApiService(IConfiguration config, ILoggerFactory loggerFactory, AzureFunctionSettings settings)
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<GraphApiService>();
            _httpClient = GraphClientFactory.Create();
            _configSettings = settings;
        }

        public GraphServiceClient GetUserGraphClient(string userAssertion)
        {
            var tenantId = _config["tenantId"];
            var clientId = _config["apiClientId"];
            var clientSecret = _config["apiClientSecret"];

            if (string.IsNullOrEmpty(tenantId) ||
                string.IsNullOrEmpty(clientId) ||
                string.IsNullOrEmpty(clientSecret))
            {
                _logger.LogError("Required settings missing: 'tenantId', 'apiClientId', and 'apiClientSecret'.");
                return null;
            }

            var onBehalfOfCredential = new OnBehalfOfCredential(
                tenantId, clientId, clientSecret, userAssertion);

            return new GraphServiceClient(onBehalfOfCredential);
        }

        public GraphServiceClient GetAppGraphClient()
        {
            try
            {
                if (_graphAppClient == null)
                {
                    var tenantId = _configSettings.TenantId;
                    var clientId = _configSettings.ClientId;
                    var clientSecret = _configSettings.ClientSecret;

                    if (string.IsNullOrEmpty(tenantId) ||
                        string.IsNullOrEmpty(clientId) ||
                        string.IsNullOrEmpty(clientSecret))
                    {
                        _logger.LogError("Required settings missing: 'TenantId', 'ClientId', and 'ClientSecret'.");
                        return null;
                    }

                    // using Azure.Identity;
                    var options = new ClientSecretCredentialOptions
                    {
                        AuthorityHost = AzureAuthorityHosts.AzurePublicCloud,
                    };

                    // https://learn.microsoft.com/dotnet/api/azure.identity.clientsecretcredential
                    var clientSecretCredential = new ClientSecretCredential(
                        tenantId, clientId, clientSecret, options);

                    _graphAppClient = new GraphServiceClient(clientSecretCredential);
                }

                return _graphAppClient;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<UserCollectionResponse> GetUsersAsync()
        {
            try
            {
                var graphClient = GetAppGraphClient();                
                _ = graphClient ?? throw new MemberAccessException("graphClient is null");
                var result = await graphClient.Users.GetAsync();

                return result;
            }
            catch (Exception)
            {
                return null;
            }

        }      
        
        public async Task<ExternalConnection> CreateConnectionAsync()
        {           
            
            var graphClient = GetAppGraphClient();
            _ = graphClient ?? throw new MemberAccessException("graphClient is null");
            return await _graphAppClient.External.Connections.PostAsync(ConnectionConfiguration.ExternalConnection);
        }
        public async Task UpdateConnectionAsync(string connectionId)
        {
            var graphClient = GetAppGraphClient();
            _ = graphClient ?? throw new MemberAccessException("graphClient is null");
            _ = connectionId ?? throw new ArgumentException("connectionId is required");
            await _graphAppClient.External.Connections[connectionId].PatchAsync(ConnectionConfiguration.ExternalConnection);
        }

        public async Task<ExternalConnection> GetConnectionAsync(string connectionId)
        {
            var graphClient = GetAppGraphClient();
            _ = graphClient ?? throw new MemberAccessException("graphClient is null");

            try
            {
                return await _graphAppClient.External.Connections[connectionId].GetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<ExternalConnectionCollectionResponse> GetExistingConnectionsAsync()
        {
            var graphClient = GetAppGraphClient();
            _ = graphClient ?? throw new MemberAccessException("graphClient is null");


            try
            {
                return await _graphAppClient.External.Connections.GetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task DeleteConnectionAsync(string connectionId)
        {
            var graphClient = GetAppGraphClient();
            _ = graphClient ?? throw new MemberAccessException("graphClient is null");
            _ = connectionId ?? throw new ArgumentException("connectionId is required");

            await _graphAppClient.External.Connections[connectionId].DeleteAsync();
        }
        public async Task RegisterSchemaAsync(string connectionId, Schema schema)
        {


            var graphClient = GetAppGraphClient();
            _ = graphClient ?? throw new MemberAccessException("graphClient is null");
            _ = _httpClient ?? throw new MemberAccessException("httpClient is null");
            _ = connectionId ?? throw new ArgumentException("connectionId is required");
            // Use the Graph SDK's request builder to generate the request URL
            var requestInfo = _graphAppClient.External
                .Connections[connectionId]
                .Schema
                .ToGetRequestInformation();

            requestInfo.SetContentFromParsable(_graphAppClient.RequestAdapter, "application/json", schema);

            // Convert the SDK request to an HttpRequestMessage
            var requestMessage = await _graphAppClient.RequestAdapter
                .ConvertToNativeRequestAsync<HttpRequestMessage>(requestInfo);
            _ = requestMessage ?? throw new Exception("Could not create native HTTP request");
            requestMessage.Method = HttpMethod.Post;
            requestMessage.Headers.Add("Prefer", "respond-async");

            // Send the request
            var responseMessage = await _httpClient.SendAsync(requestMessage) ??
                throw new Exception("No response returned from API");

            if (responseMessage.IsSuccessStatusCode)
            {
                // The operation ID is contained in the Location header returned
                // in the response
                var operationId = responseMessage.Headers.Location?.Segments.Last() ??
                    throw new Exception("Could not get operation ID from Location header");
                await WaitForOperationToCompleteAsync(connectionId, operationId);
            }
            else
            {
                throw new ServiceException("Registering schema failed",
                    responseMessage.Headers, (int)responseMessage.StatusCode);
            }
        }
        private async Task WaitForOperationToCompleteAsync(string connectionId, string operationId)
        {
            var graphClient = GetAppGraphClient();
            _ = graphClient ?? throw new MemberAccessException("graphClient is null");

            do
            {
                var operation = await _graphAppClient.External
                    .Connections[connectionId]
                    .Operations[operationId]
                    .GetAsync();

                if (operation?.Status == ConnectionOperationStatus.Completed)
                {
                    return;
                }
                else if (operation?.Status == ConnectionOperationStatus.Failed)
                {
                    throw new ServiceException($"Schema operation failed: {operation?.Error?.Code} {operation?.Error?.Message}");
                }

                // Wait 5 seconds and check again
                await Task.Delay(5000);
            } while (true);
        }
        public async Task<Schema> GetSchemaAsync(string connectionId)
        {
            var graphClient = GetAppGraphClient();
            _ = graphClient ?? throw new MemberAccessException("graphClient is null");
            _ = connectionId ?? throw new ArgumentException("connectionId is null");

            return await _graphAppClient.External
                .Connections[connectionId]
                .Schema
                .GetAsync();
        }
        public async Task AddOrUpdateItemAsync(string connectionId, ExternalItem item)
        {
            var graphClient = GetAppGraphClient();
            _ = graphClient ?? throw new MemberAccessException("graphClient is null");
            _ = connectionId ?? throw new ArgumentException("connectionId is null");

            await _graphAppClient.External
                .Connections[connectionId]
                .Items[item.Id]
                .PutAsync(item);
        }
    }
}

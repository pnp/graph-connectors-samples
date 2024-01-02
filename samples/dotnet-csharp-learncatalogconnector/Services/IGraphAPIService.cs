// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ExternalConnectors;
using System.Threading.Tasks;

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
}

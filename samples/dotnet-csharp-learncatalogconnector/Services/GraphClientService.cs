// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Threading.Tasks;
using System;

namespace O365C.GraphConnector.MicrosoftLearn.Services
{
    public class GraphClientService 
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private GraphServiceClient _graphClient;

        public GraphClientService(IConfiguration config, ILoggerFactory loggerFactory)
        //GraphServiceClient graphServiceClient
        {
            _config = config;
            _logger = loggerFactory.CreateLogger<GraphClientService>();
            //_graphClient = graphServiceClient;
        }

        

        /*
            This function creates and returns a GraphServiceClient object that can be used to make requests on behalf of a user.
            It takes a user assertion as a parameter, which is used to authenticate the user.
            The function retrieves the required configuration values from the app settings.
            If any of the required configuration values are missing, an error message is logged and null is returned.
            Otherwise, an instance of OnBehalfOfCredential is created using the configuration values and the user assertion.
            Finally, a new GraphServiceClient is created using the OnBehalfOfCredential and returned.
        */
        // public GraphServiceClient GetUserGraphClient(string userAssertion)
        // {
        //     var tenantId = _config["tenantId"];
        //     var clientId = _config["apiClientId"];
        //     var clientSecret = _config["apiClientSecret"];

        //     if (string.IsNullOrEmpty(tenantId) ||
        //         string.IsNullOrEmpty(clientId) ||
        //         string.IsNullOrEmpty(clientSecret))
        //     {
        //         _logger.LogError("Required settings missing: 'tenantId', 'apiClientId', and 'apiClientSecret'.");
        //         return null;
        //     }

        //     var onBehalfOfCredential = new OnBehalfOfCredential(
        //         tenantId, clientId, clientSecret, userAssertion);

        //     return new GraphServiceClient(onBehalfOfCredential);
        // }

        /*
            This function returns a GraphServiceClient object that can be used to make requests on behalf of the application.
            It retrieves the required configuration values from the app settings.
            If any of the required configuration values are missing, an error message is logged and null is returned.
            Otherwise, a new instance of ClientSecretCredential is created using the configuration values.
            Finally, a new GraphServiceClient is created using the ClientSecretCredential and returned.
        */
        // public GraphServiceClient GetAppGraphClient()
        // {
        //     if (_appGraphClient == null)
        //     {
        //         var tenantId = _config["TenantId"];
        //         var clientId = _config["ClientId"];
        //         var clientSecret = _config["ClientSecret"];

        //         if (string.IsNullOrEmpty(tenantId) ||
        //             string.IsNullOrEmpty(clientId) ||
        //             string.IsNullOrEmpty(clientSecret))
        //         {
        //             _logger.LogError("Required settings missing: 'TenantId', 'ClientId', and 'ClientSecret'.");
        //             return null;
        //         }

        //         var clientSecretCredential = new ClientSecretCredential(
        //             tenantId, clientId, clientSecret);

        //         _appGraphClient = new GraphServiceClient(clientSecretCredential);
        //     }

        //     return _appGraphClient;
        // }


        public async Task<UserCollectionResponse> GetUsersAsync()
        {
            try
            {                
                var result = await _graphClient.Users.GetAsync();

                return result;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}

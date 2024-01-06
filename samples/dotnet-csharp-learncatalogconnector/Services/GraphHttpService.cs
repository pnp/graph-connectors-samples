//Create GrapHttpService.cs file under Services folder and add the below code   


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ExternalConnectors;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using O365C.GraphConnector.MicrosoftLearn.Models;
using O365C.GraphConnector.MicrosoftLearn.Util;

namespace O365C.GraphConnector.MicrosoftLearn.Services
{

    public interface IGraphHttpService
    {
        Task<UserCollectionResponse> GetUsersAsync();
        Task<ExternalConnection> CreateConnectionAsync(string connectorId, string connectorTicket);

        Task<ExternalConnection> GetConnectionAsync(string connectionId);

        Task<Schema> GetSchemaAsync(string connectionId);

        Task CreateSchemaAsync();

        Task CreateItemAsync(Module module);

        Task DeleteConnectionAsync(string connectionId);

    }
    public class GraphHttpService : IGraphHttpService
    {

        private const string GraphBaseUrl = "https://graph.microsoft.com/v1.0";
        private readonly HttpClient _client;
        private readonly IConfidentialClientApplication _confidentialClientApp;
        private readonly IAccessTokenProvider _accessTokenProvider;
        private static string _accessToken { get; set; } // Static property for AccessToken

        public GraphHttpService(IConfidentialClientApplication confidentialClientApp, IHttpClientFactory httpClientFactory, IAccessTokenProvider accessTokenProvider)
        {
            _confidentialClientApp = confidentialClientApp;
            _client = httpClientFactory.CreateClient();
            _accessTokenProvider = accessTokenProvider;
        }

        public async Task<UserCollectionResponse> GetUsersAsync()
        {
            try
            {

                var AccessToken = await _accessTokenProvider.GetAccessTokenAsync();
                string endpoint = $"{GraphBaseUrl}/users";

                using (var request = new HttpRequestMessage(HttpMethod.Get, endpoint))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AccessToken);

                    var response = _client.SendAsync(request).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var stringResult = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<UserCollectionResponse>(stringResult);
                        return result;
                    }
                    else
                    {
                        throw new Exception($"Error getting users: {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                //Log exception
                throw new Exception($"Error getting users: {ex.Message}");

            }
        }

        public async Task<ExternalConnection> CreateConnectionAsync(string connectorId, string connectorTicket)
        {
            try
            {
                var accessToken = await _accessTokenProvider.GetAccessTokenAsync();
                string endpoint = $"{GraphBaseUrl}/external/connections";

                var adaptiveCard = File.ReadAllText(CommonConstants.ResultLayoutFilePath);

                using (var request = new HttpRequestMessage(HttpMethod.Post, endpoint))
                {
                    //Headers
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (!string.IsNullOrEmpty(connectorTicket))
                    {
                        request.Headers.Add("GraphConnectors-Ticket", connectorTicket);
                    }


                    var payload = new
                    {
                        description = "This is a connector created by Microsoft Learn",
                        id = ConnectionConfiguration.ConnectionID,
                        name = "Microsoft Learn Connector",
                        connectorId = !string.IsNullOrEmpty(connectorId) ? connectorId : null,
                        searchSettings = new
                        {
                            searchResultTemplates = new[]
                            {
                                new
                                {
                                    id = "SrcLearnCat",
                                    priority = 1,
                                    layout = new {
                                        additionalProperties = adaptiveCard
                                    }
                                }
                            }
                        },
                        activitySettings = new
                        {
                            urlToItemResolvers = new[]
                            {
                                new
                                {
                                    @odata_type = "#microsoft.graph.externalConnectors.itemIdResolver",
                                    urlMatchInfo = new
                                    {
                                        baseUrls = new[] { CommonConstants.BaseUrl },
                                        urlPattern = "/training/modules/(?<slug>[^/]+)/?",
                                    },
                                    itemId = "{slug}",
                                    priority = 1
                                }
                            }
                        }
                    };

                    string jsonContent = JsonConvert.SerializeObject(payload);

                    jsonContent = jsonContent.ToString().Replace("odata_type", "@odata.type");

                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = _client.SendAsync(request).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var stringResult = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<ExternalConnection>(stringResult);
                        return result;
                    }
                    else
                    {
                        throw new Exception($"Error creating connection: {response.ReasonPhrase}");
                    }

                }
            }
            catch (Exception ex)
            {
                //Log exception
                throw new Exception($"Error creating connection: {ex.Message}");

            }
        }

        public async Task<ExternalConnection> GetConnectionAsync(string connectionId)
        {
            try
            {

                var accessToken = await _accessTokenProvider.GetAccessTokenAsync();

                string endpoint = $"{GraphBaseUrl}/external/connections/{connectionId}";

                using (var request = new HttpRequestMessage(HttpMethod.Get, endpoint))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    var response = _client.SendAsync(request).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var stringResult = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<ExternalConnection>(stringResult);
                        return result;
                    }
                    else
                    {
                        //If not found then return null
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            return null;
                        }
                        else
                        {
                            throw new Exception($"Error getting connection: {response.ReasonPhrase}");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //Log exception
                throw new Exception($"Error getting connection: {ex.Message}");

            }

        }

        public async Task DeleteConnectionAsync(string connectionId)
        {
            try
            {

                var accessToken = await _accessTokenProvider.GetAccessTokenAsync();

                string endpoint = $"{GraphBaseUrl}/external/connections/{connectionId}";

                using (var request = new HttpRequestMessage(HttpMethod.Delete, endpoint))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var response = _client.SendAsync(request).Result;                    
                }
            }
            catch (Exception ex)
            {
                //Log exception
                throw new Exception($"Error deleting connection: {ex.Message}");

            }

        }

        public async Task<Schema> GetSchemaAsync(string connectionId)
        {
            try
            {
                var accessToken = await _accessTokenProvider.GetAccessTokenAsync();

                string endpoint = $"{GraphBaseUrl}/external/connections/{connectionId}/schema";

                using (var request = new HttpRequestMessage(HttpMethod.Get, endpoint))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    var response = _client.SendAsync(request).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var stringResult = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<Schema>(stringResult);
                        return result;
                    }
                    else
                    {
                        //If not found then return null
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            return null;
                        }
                        else
                        {
                            throw new Exception($"Error getting schema: {response.ReasonPhrase}");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //Log exception
                throw new Exception($"Error getting schema: {ex.Message}");

            }
        }
        public async Task CreateSchemaAsync()
        {

            try
            {
                var accessToken = await _accessTokenProvider.GetAccessTokenAsync();
                string endpoint = $"{GraphBaseUrl}/external/connections/{ConnectionConfiguration.ConnectionID}/schema";
                using var request = new HttpRequestMessage(HttpMethod.Patch, endpoint);
                //Headers
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                var payload = new
                {
                    baseType = "microsoft.graph.externalItem",
                    properties = new object[]
                       {
                                new
                                {
                                    name = "Summary",
                                    type = "String",
                                    isQueryable = "true",
                                    isSearchable = "true",
                                    isRetrievable = "true"
                                },
                                new
                                {
                                    name = "Levels",
                                    type = "String",
                                    isRetrievable = "true"
                                },
                                new
                                {
                                    name = "Roles",
                                    type = "String",
                                    isRetrievable = "true"
                                },
                                new
                                {
                                    name = "Products",
                                    type = "String",
                                    isRetrievable = "true"
                                },
                                new
                                {
                                    name = "Subjects",
                                    type = "String",
                                    isRetrievable = "true"
                                },
                                new
                                {
                                    name = "Uid",
                                    type = "String",
                                    isRetrievable = "true"
                                },
                                new
                                {
                                    name = "Title",
                                    type = "String",
                                    isQueryable = "true",
                                    isSearchable = "true",
                                    isRetrievable = "true",
                                    labels = new[] { "Title" }
                                },
                                new
                                {
                                    name = "Duration",
                                    type = "String",
                                    isRetrievable = "true"
                                },
                                new
                                {
                                    name = "Rating",
                                    type = "String",
                                    isRetrievable = "true"
                                },
                                new
                                {
                                    name = "IconUrl",
                                    type = "String",
                                    isRetrievable = "true"
                                },
                                new
                                {
                                    name = "SocialImageUrl",
                                    type = "String",
                                    isRetrievable = "true"
                                },
                                 new
                                {
                                    name = "LastModified",
                                    type = "DateTime",
                                    isQueryable = "true",
                                    isRefinable = "true",
                                    isRetrievable = "true",
                                    labels = new[] { "LastModifiedDateTime" }
                                },
                                new
                                {
                                    name = "Url",
                                    type = "String",
                                    isRetrievable = "true"
                                },
                                new
                                {
                                    name = "Units",
                                    type = "String",
                                    isRetrievable = "true"
                                },
                                new
                                {
                                    name = "NumberOfUnits",
                                    type = "String",
                                    isRetrievable = "true"
                                }

                        }
                };


                string jsonContent = JsonConvert.SerializeObject(payload);

                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = _client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    // The operation ID is contained in the Location header returned
                    // in the response
                    var operationId = response.Headers.Location?.Segments.Last() ??
                        throw new Exception("Could not get operation ID from Location header");
                    await WaitForOperationToCompleteAsync(accessToken, ConnectionConfiguration.ConnectionID, operationId);

                }
                else
                {
                    //throw new Exception($"Error creating schema: {response.ReasonPhrase}");
                    throw new ServiceException("creating schema failed", response.Headers, (int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                //Log exception
                throw new Exception($"Error creating schema: {ex.Message}");

            }
        }

        public async Task CreateItemAsync(Module module)
        {
            if (_accessToken == null)
            {
                _accessToken = await _accessTokenProvider.GetAccessTokenAsync();
            }
            var accessToken = _accessToken;

            var itemId = Regex.Replace(module.Uid, "[^a-zA-Z0-9-]", "");
            string endpoint = $"{GraphBaseUrl}/external/connections/{ConnectionConfiguration.ConnectionID}/items/{itemId}";
            using var request = new HttpRequestMessage(HttpMethod.Put, endpoint);
            //Headers
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            var payload = new
            {
                id = itemId,
                acl = new object[]
                {
                        new
                        {
                            type = "everyone",
                            value = "everyone",
                            accessType = "grant"
                        }
                },
                properties = new
                {
                    Summary = module.Summary ?? string.Empty,
                    Levels = string.Join(",", module.Levels ?? new List<string>()),
                    Roles = string.Join(',', module.Roles ?? new List<string>()),
                    Products = string.Join(",", module.Products ?? new List<string>()),
                    Subjects = string.Join(",", module.Subjects ?? new List<string>()),
                    Uid = module.Uid ?? "",
                    Title = module.Title ?? "",
                    Duration = module.Duration.ToString() ?? "",
                    Rating = module.Rating != null ? GetStarRating((int)module.Rating.Average) : "",
                    IconUrl = module.IconUrl ?? "",
                    SocialImageUrl = module.SocialImageUrl ?? "",
                    LastModified = module.LastModified,
                    Url = module.Url ?? "",
                    Units = string.Join(",", module.Units ?? new List<string>()),
                    NumberOfUnits = module.NumberOfUnits.ToString() ?? ""
                },
                content = new
                {
                    value = module.Summary ?? "",
                    type = "text"
                },
            };


            string jsonContent = JsonConvert.SerializeObject(payload);

            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = _client.SendAsync(request).Result;

        }

        private async Task WaitForOperationToCompleteAsync(string accessToken, string connectionId, string operationId)
        {

            do
            {

                string endpoint = $"{GraphBaseUrl}/external/connections/{connectionId}/operations/{operationId}";

                using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                //Headers
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = _client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    var stringResult = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<ConnectionOperation>(stringResult);

                    if (result.Status == ConnectionOperationStatus.Completed)
                    {
                        return;
                    }
                    else if (result.Status == ConnectionOperationStatus.Failed)
                    {
                        throw new ServiceException($"Schema operation failed: {result.Error?.Code} {result.Error?.Message}");
                    }
                }
                else
                {
                    throw new Exception($"Error getting operation status: {response.ReasonPhrase}");
                }
                // Wait 5 seconds and check again
                await Task.Delay(5000);
            } while (true);
        }
        private static object GetStarRating(int count)
        {
            switch (count)
            {
                case 0:
                    return "";
                case 1:
                    return "⭐️";
                case 2:
                    return "⭐️⭐️";
                case 3:
                    return "⭐️⭐️⭐️";
                case 4:
                    return "⭐️⭐️⭐️⭐️ ";
                case 5:
                    return "⭐️⭐️⭐️⭐️⭐️";
                default:
                    return "";
            }
        }

    }
}
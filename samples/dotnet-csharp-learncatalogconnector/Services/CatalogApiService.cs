using Newtonsoft.Json;
using O365C.GraphConnector.MicrosoftLearn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;
using Module = O365C.GraphConnector.MicrosoftLearn.Models.Module;
using System.Net.Http;

namespace O365C.GraphConnector.MicrosoftLearn.Services
{
    public class CatalogApiService : ICatalogApiService
    {
        private readonly HttpClient _client;
        private readonly string _apiUrl = "https://learn.microsoft.com/api/catalog"; // Replace with the actual API base URL
        public CatalogApiService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }


        /// <summary>
        /// Returns the Learn catalog for the specified locale with filters
        /// </summary>
        /// <param name="locale">Optional language locale (en-us)</param>
        /// <param name="filters">Optional filters to apply to the results</param>
        /// <returns>Learn Catalog of modules, paths, and relationships</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<LearnCatalog> GetCatalogAsync(string? locale = null, CatalogFilter? filters = null)
        {
            var endpoint = _apiUrl;

            var parameters = new List<string>();

            if (!string.IsNullOrWhiteSpace(locale))
            {
                parameters.Add($"locale={WebUtility.HtmlEncode(locale)}");
            }
            if (filters?.Types != null)
            {
                var value = filters.Types.ToString() ?? string.Empty;
                var values = value.Split(',');
                value = string.Join(',', values.Select(v =>
                {
                    v = v.Trim();
                    v = char.ToLower(v[0]) + v[1..];
                    return v;
                }));
                if (!string.IsNullOrWhiteSpace(value))
                    parameters.Add($"type={WebUtility.HtmlEncode(value)}");
            }
            if (filters?.Uids?.Count > 0)
                parameters.Add($"uid={WebUtility.HtmlEncode(string.Join(',', filters.Uids.Where(s => !string.IsNullOrWhiteSpace(s))))}");
            if (!string.IsNullOrWhiteSpace(filters?.LastModifiedExpression))
                parameters.Add($"last_modified={ConvertExpression(filters.LastModifiedExpression)}");
            if (!string.IsNullOrWhiteSpace(filters?.PopularityExpression))
                parameters.Add($"popularity={ConvertExpression(filters.PopularityExpression)}");
            if (filters?.Levels?.Count > 0)
                parameters.Add($"level={WebUtility.HtmlEncode(string.Join(',', filters.Levels.Where(s => !string.IsNullOrWhiteSpace(s))))}");
            if (filters?.Roles?.Count > 0)
                parameters.Add($"role={WebUtility.HtmlEncode(string.Join(',', filters.Roles.Where(s => !string.IsNullOrWhiteSpace(s))))}");
            if (filters?.Products?.Count > 0)
                parameters.Add($"product={WebUtility.HtmlEncode(string.Join(',', filters.Products.Where(s => !string.IsNullOrWhiteSpace(s))))}");
            if (filters?.Subjects?.Count > 0)
                parameters.Add($"subject={WebUtility.HtmlEncode(string.Join(',', filters.Subjects.Where(s => !string.IsNullOrWhiteSpace(s))))}");

            if (parameters.Count > 0)
            {
                endpoint += "?" + string.Join('&', parameters);
            }



            //using var client = new HttpClient();
            var response = await _client.GetAsync(endpoint).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (errorText.Contains("message"))
                {
                    var error = JsonConvert.DeserializeObject<ErrorResponse>(errorText);
                    if (error != null)
                    {
                        throw new InvalidOperationException($"{error.ErrorCode}: {error.Message}");
                    }
                }

                throw new InvalidOperationException(
                    $"Failed to retrieve catalog information - {response.StatusCode}: {errorText}");
            }

            var jsonText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var catalog = JsonConvert.DeserializeObject<LearnCatalog>(jsonText,
                new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                }) ?? throw new InvalidOperationException(
                    "Unable to parse results from Learn Catalog - possibly outdated schema?");

            // Fill in path ratings if the system didn't supply it.
            foreach (var path in catalog.LearningPaths.Where(p => p.Rating?.Count == 0))
            {
                var modules = catalog.ModulesForPath(path).ToList();
                if (modules.Any(m => m.Rating?.Count > 0))
                {
                    path.Rating = new Rating
                    {
                        Count = modules.Sum(m => m.Rating?.Count ?? 0),
                        Average = modules.Where(m => m.Rating != null)
                                         .Average(m => m.Rating!.Average)
                    };
                }
            }
            
            return catalog;
        }


        /// <summary>
        /// Returns the modules from the Catalog API
        /// </summary>
        /// <param name="locale">Optional locale</param>
        /// <param name="filter">Optional query filters to apply</param>
        /// <returns>List of available Microsoft Learn modules</returns>
        public async Task<List<Module>> GetModulesAsync(string? locale = null, CatalogFilter? filter = null)
        {
            filter ??= new CatalogFilter();
            var result = await GetCatalogAsync(locale, filter);
            var modules = result.Modules;
            var units = result.Units;
            var roles = result.Roles;

            // Loop through each module and then get the matching unit from the units list and add the unit title to the module object
            foreach (var module in modules)
            {
                //Initialize the list of strings array
                List<string> moduleUnits = new List<string>();

                foreach (var unit in module.Units)
                {
                    // Extract the text after the last dot (.)
                    var matchingUnit = units.FirstOrDefault(u => u.Uid == unit);
                    if (matchingUnit != null)
                    {
                        var unitTitle = matchingUnit.Title;
                        moduleUnits.Add(unitTitle);
                    }
                }
                module.Units = moduleUnits;

                List<string> moduleRoles = new List<string>();
                //loop through the roles and get the matching role from the roles list and add the role title to the module object 
                foreach (var role in module.Roles)
                {
                    var matchingRole = roles.FirstOrDefault(r => r.Id == role);
                    if (matchingRole != null)
                    {
                        var roleName = matchingRole.Name;
                        moduleRoles.Add(roleName);
                    }
                }
                module.Roles = moduleRoles;
            }

            return modules;
        }

        // public async Task<List<Module>> GetAllModules()
        // {
        //     //Add try catch block with in this method and return empty list if any exception occurs    
        //     try
        //     {
        //         // Construct the API request URL with necessary query parameters                
        //         //var requestUrl = $"{_apiUrl}?locale=en-us&type=modules";
        //         var requestUrl = $"{_apiUrl}?locale=en-us";

        //         var response = await _httpClient.GetAsync(requestUrl);
        //         response.EnsureSuccessStatusCode();

        //         // Read the JSON response as a string
        //         string jsonResponse = await response.Content.ReadAsStringAsync();

        //         // Deserialize JSON string to ModuleResponse object
        //         ModuleResponse moduleResponse = System.Text.Json.JsonSerializer.Deserialize<ModuleResponse>(jsonResponse);

        //         // Access the list of modules
        //         List<Module> modules = moduleResponse.modules;

        //         //Take top 50 modules
        //         //modules = modules.Take(30).ToList();



        //         //loop through the modules and get the unit details for each module and add to the module object as a list of strings 
        //         foreach (var module in modules)
        //         {
        //             //Initialize the list of strings array
        //             List<string> moduleUnits = new List<string>();

        //             foreach (var unit in module.Units)
        //             {
        //                 // Extract the text after the last dot (.)
        //                 var unitTitle = unit.Substring(unit.LastIndexOf('.') + 1);

        //                 // Convert unitTitle to Title case
        //                 var textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
        //                 unitTitle = textInfo.ToTitleCase(unitTitle);

        //                 moduleUnits.Add(unitTitle);
        //             }
        //             module.Units = moduleUnits;
        //         }

        //         return modules;

        //     }
        //     catch (Exception ex)
        //     {
        //         return new List<Module>();
        //     }

        // }
        // public async Task<List<Module>> GetModulesWithPopularityScoreAbove(double score)
        // {
        //     //Add try catch block with in this method and return empty list if any exception occurs    
        //     try
        //     {
        //         // Construct the API request URL with necessary query parameters
        //         //var requestUrl = $"{_apiUrl}?locale=en-us&type=modules,units,roles&popularity>{score}&role=developer";
        //         var requestUrl = $"{_apiUrl}?locale=en-us&type=modules";

        //         var response = await _httpClient.GetAsync(requestUrl);
        //         response.EnsureSuccessStatusCode();

        //         // Read the JSON response as a string
        //         string jsonResponse = await response.Content.ReadAsStringAsync();

        //         // Deserialize JSON string to ModuleResponse object
        //         ModuleResponse moduleResponse = System.Text.Json.JsonSerializer.Deserialize<ModuleResponse>(jsonResponse);

        //         // Access the list of modules
        //         List<Module> modules = moduleResponse.modules;

        //         //Take top 50 modules
        //         //modules = modules.Take(30).ToList();



        //         //loop through the modules and get the unit details for each module and add to the module object as a list of strings 
        //         foreach (var module in modules)
        //         {
        //             //Initialize the list of strings array
        //             List<string> moduleUnits = new List<string>();

        //             foreach (var unit in module.Units)
        //             {
        //                 // Extract the text after the last dot (.)
        //                 var unitTitle = unit.Substring(unit.LastIndexOf('.') + 1);

        //                 // Convert unitTitle to Title case
        //                 var textInfo = new System.Globalization.CultureInfo("en-US", false).TextInfo;
        //                 unitTitle = textInfo.ToTitleCase(unitTitle);

        //                 moduleUnits.Add(unitTitle);
        //             }
        //             module.Units = moduleUnits;
        //         }

        //         return modules;


        //         // //loop through the modules and get the unit details for each module and add to the module object as a list of strings 
        //         // foreach(var module in modules)
        //         // {
        //         //     //Initialize the list of strings array
        //         //     List<string> moduleUnits = new List<string>();

        //         //     foreach(var unit in module.units)
        //         //     {
        //         //         // Construct the API request URL with necessary query parameters
        //         //         var unitRequestUrl = $"{_apiUrl}?uid={unit}&locale=en-us&include=units";

        //         //         var unitResponse = await _httpClient.GetAsync(unitRequestUrl);
        //         //         unitResponse.EnsureSuccessStatusCode();

        //         //         // Read the JSON response as a string
        //         //         string unitJsonResponse = await unitResponse.Content.ReadAsStringAsync();

        //         //         // Deserialize JSON string to ModuleResponse object
        //         //         Root root = System.Text.Json.JsonSerializer.Deserialize<Root>(unitJsonResponse);

        //         //         foreach(var item in root.units)
        //         //         {
        //         //             moduleUnits.Add(item.title);
        //         //         }
        //         //     }
        //         //     module.units = moduleUnits;
        //         // }               

        //         // return modules;
        //     }
        //     catch (Exception ex)
        //     {
        //         return new List<Module>();
        //     }

        // }

        /// <summary>
        /// Convert the given expression to a valid expression for the catalog API.
        /// </summary>
        /// <param name="expression">Input expression</param>
        /// <returns>Filter for the catalog API</returns>
        private static string ConvertExpression(string expression)
        {
            expression = expression.Trim()
                .Replace(">=", "gte ")
                .Replace("<=", "lte ")
                .Replace("=", "eq ")
                .Replace(">", "gt ")
                .Replace("<", "lt ")
                .Replace("  ", " ");

            return WebUtility.HtmlEncode(expression);

        }

    }
}

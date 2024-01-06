using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using O365C.GraphConnector.MicrosoftLearn;
using O365C.GraphConnector.MicrosoftLearn.Models;
using O365C.GraphConnector.MicrosoftLearn.Services;
using System;
using System.Text.Json;
using Microsoft.Identity.Client;

[assembly: FunctionsStartup(typeof(Startup))]
namespace O365C.GraphConnector.MicrosoftLearn
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            // Get the configuration from the app settings
            var config = builder.GetContext().Configuration;
            var azureFunctionSettings = new AzureFunctionSettings();
            config.Bind(azureFunctionSettings);
            // Add our configuration class
            builder.Services.AddSingleton(options => { return azureFunctionSettings; });

            // Register your services
            builder.Services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>();

            builder.Services.AddHttpClient();
            // builder.Services.AddHttpClient("GraphApi", client =>
            // {
            //     client.BaseAddress = new Uri("https://graph.microsoft.com/v1.0/");
            // });

          


            // Configure MSAL
            builder.Services.AddSingleton(provider =>
            {
                var clientId = azureFunctionSettings.ClientId; // Replace with your app's client ID
                var clientSecret = azureFunctionSettings.ClientSecret; // Replace with your app's client secret
                var authority = "https://login.microsoftonline.com/common";

                return ConfidentialClientApplicationBuilder
                    .Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(new Uri(authority))
                    .Build();
            });

            // Configure custom services
            builder.Services.AddSingleton<IGraphHttpService, GraphHttpService>();

            builder.Services.AddHttpClient();
            // builder.Services.AddHttpClient("GraphApi", client =>
            // {
            //     client.BaseAddress = new Uri("https://graph.microsoft.com/v1.0/");
            // });



            builder.Services.Configure<JsonSerializerOptions>(options =>
            {
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });



            // Add the Microsoft Graph client to the DI container
            builder.Services.AddSingleton<IGraphAPIService, GraphApiService>();

            // Add the Catalog API Service client to the DI container
            builder.Services.AddSingleton<ICatalogApiService, CatalogApiService>();

            // Initialize the Graph client for the app using the Client Credential flow
            //builder.Services.AddSingleton<GraphServiceClient>(GraphHelper.InitializeGraphClient(azureFunctionSettings.ClientId, azureFunctionSettings.TenantId, azureFunctionSettings.ClientSecret));

            builder.Services.AddSingleton<ExecutionContextOptions>();

        }
    }
}

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using O365C.GraphConnector.MicrosoftLearn;
using O365C.GraphConnector.MicrosoftLearn.Models;
using O365C.GraphConnector.MicrosoftLearn.Services;
using System.Text.Json;

[assembly: FunctionsStartup(typeof(Startup))]
namespace O365C.GraphConnector.MicrosoftLearn
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();             

            var config = builder.GetContext().Configuration;
            var azureFunctionSettings = new AzureFunctionSettings();
            config.Bind(azureFunctionSettings);

            // Add our configuration class
            builder.Services.AddSingleton(options => { return azureFunctionSettings; });

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

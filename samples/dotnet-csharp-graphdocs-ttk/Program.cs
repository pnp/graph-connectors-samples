using Azure.Core.Pipeline;
using Azure.Identity;
using Azure.Storage.Queues;
using GraphDocsConnector;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder
            .AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, "appsettings.json"), optional: true, reloadOnChange: false)
            .AddJsonFile(Path.Combine(context.HostingEnvironment.ContentRootPath, $"appsettings.{context.HostingEnvironment.EnvironmentName}.json"), optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();
    })
    .ConfigureServices((context, s) =>
    {
        var config = context.Configuration;

        s.AddApplicationInsightsTelemetryWorkerService();
        s.ConfigureFunctionsApplicationInsights();
        s.AddSingleton(s =>
        {
            var handler = Utils.GetHttpClientHandler();
            var options = new ClientSecretCredentialOptions
            {
                Transport = new HttpClientTransport(handler)
            };

            var clientId = config["Entra:ClientId"];
            var clientSecret = config["Entra:ClientSecret"];
            var tenantId = config["Entra:TenantId"];

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);
            var handlers = GraphClientFactory.CreateDefaultHandlers();
            var httpClient = GraphClientFactory.Create(handlers, proxy: Utils.GetWebProxy());

            return new GraphServiceClient(httpClient, credential);
        });
        s.AddSingleton(s =>
        {
            var clientId = config["Entra:ClientId"];
            var clientSecret = config["Entra:ClientSecret"];
            var tenantId = config["Entra:TenantId"];
            var scopes = (config["DocumentsApi:Scopes"] ?? "").Split(' ');

            var httpClientHandler = Utils.GetHttpClientHandler();
            var httpClientFactory = new CustomHttpClientFactory(httpClientHandler);

            var app = ConfidentialClientApplicationBuilder.Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                    .WithHttpClientFactory(httpClientFactory)
                    .Build();

            var authenticationHandler = new AuthenticationDelegatingHandler(app, scopes)
            {
                InnerHandler = Utils.GetHttpClientHandler()
            };
            var httpClient = new HttpClient(authenticationHandler);

            return new DocumentsServiceClient(httpClient);
        });
        s.AddAzureClients(configureClients =>
        {
            configureClients
                .AddQueueServiceClient(config.GetValue<string>("AzureWebJobsStorage"))
                .ConfigureOptions(options => options.MessageEncoding = QueueMessageEncoding.Base64);
            configureClients
                .AddTableServiceClient(config.GetValue<string>("AzureWebJobsStorage"));
        });
    })
    .Build();

host.Run();

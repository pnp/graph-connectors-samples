using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using O365C.GraphConnector.MicrosoftLearn.Services;
using System.Collections.Generic;
using Microsoft.Graph.Models.ExternalConnectors;
using O365C.GraphConnector.MicrosoftLearn.Util;
using O365C.GraphConnector.MicrosoftLearn.Models;

namespace O365C.GraphConnector.MicrosoftLearn
{
    public class OnDemandContent
    {

        private readonly IGraphHttpService _graphHttpService;

        private readonly ICatalogApiService _catalogApiService;
        private readonly ILogger _logger;

        private readonly IAccessTokenProvider _accessTokenProvider;

        public OnDemandContent(
            ILoggerFactory loggerFactory,
            ICatalogApiService catalogApiService,
            IGraphHttpService graphHttpService,
            IAccessTokenProvider accessTokenProvider)
        {

            _logger = loggerFactory.CreateLogger<OnDemandContent>();
            _catalogApiService = catalogApiService;
            _graphHttpService = graphHttpService;
            _accessTokenProvider = accessTokenProvider;
        }


        [FunctionName("OnDemandContent")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {

            log.LogInformation($"OnDemandContent HTTP trigger function executed at: {DateTime.Now}");



            //Delete connection
            //await _graphAPIService.DeleteConnectionAsync(ConnectionConfiguration.ConnectionID);

            CommonConstants.ResultLayoutFilePath = Path.Combine(context.FunctionAppDirectory, "Assets", "resultLayout.json");

            //Create connection
            //await CreateConnection(log);

            //Create schema
           // await CreateSchema(log);

            //Ingest content
            //await IngestContent(log);

            //Return OK
            return new OkObjectResult("All steps are completed");

        }


        public async Task CreateConnection(ILogger log)
        {
            _ = _graphHttpService ?? throw new MemberAccessException("graphHttpService is null");

            try
            {
                ExternalConnection connection = await _graphHttpService.GetConnectionAsync(ConnectionConfiguration.ConnectionID);

                if (connection == null)
                {
                    log.LogInformation("No connection was found, creating it...");
                    connection = await _graphHttpService.CreateConnectionAsync("", "");
                    log.LogInformation("Done");
                }
                else
                {
                    log.LogInformation("Connection already exists, Skipping");
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error creating connection");
                throw;
            }
        }

        public async Task CreateSchema(ILogger log)
        {
            _ = _graphHttpService ?? throw new MemberAccessException("graphAPIService is null");

            try
            {
                //Get Schema    
                log.LogInformation("Getting Schema");
                Schema schema = await _graphHttpService.GetSchemaAsync(ConnectionConfiguration.ConnectionID);
                log.LogInformation("Got Schema");
                //Schema is null then provision schema  
                if (schema == null)
                {
                    log.LogInformation("Provisioning Schema, this will take a few minutes");
                    await _graphHttpService.CreateSchemaAsync();
                    log.LogInformation("Provisioned");
                }
                else
                {
                    log.LogInformation("Schema already exists. Skipping");
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error creating schema");
                throw;
            }

        }

        private async Task IngestContent(ILogger log)
        {
            //load modules
            log.LogInformation("Loading modules...");
            List<Module> modules = await _catalogApiService.GetModulesAsync();
            log.LogInformation("Loaded");          

            foreach (var module in modules)
            {
                log.LogInformation(string.Format("Loading item {0}...", module.Uid));
                log.LogInformation($"--------------------------------------------------");
                try
                {
                    await _graphHttpService.CreateItemAsync(module);
                    log.LogInformation("DONE");
                }
                catch (Exception ex)
                {
                    log.LogError("ERROR");
                    log.LogError(ex.Message);
                    throw;
                }
            }
        }


    }
}
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using O365C.GraphConnector.MicrosoftLearn.Services;
using System.Collections.Generic;
using Microsoft.Graph.Models.ExternalConnectors;
using System.Linq;
using O365C.GraphConnector.MicrosoftLearn.Util;
using O365C.GraphConnector.MicrosoftLearn.Helpers;
using Microsoft.Extensions.Configuration;
using O365C.GraphConnector.MicrosoftLearn.Models;

namespace O365C.GraphConnector.MicrosoftLearn
{
    public class OnDemandContent
    {

        private readonly IGraphAPIService _graphAPIService;
        private readonly ICatalogApiService _catalogApiService;
        private readonly ILogger _logger;

        public OnDemandContent(
            IGraphAPIService graphAPIService,
            ILoggerFactory loggerFactory, ICatalogApiService catalogApiService)
        {
            _graphAPIService = graphAPIService;
            _logger = loggerFactory.CreateLogger<OnDemandContent>();
            _catalogApiService = catalogApiService;
        }


        [FunctionName("OnDemandContent")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {
                        
            log.LogInformation($"OnDemandContent HTTP trigger function executed at: {DateTime.Now}");

            CommonConstants.ResultLayoutFilePath = Path.Combine(context.FunctionAppDirectory, "Cards", "resultLayout.json");

            //Create connection
            await CreateOrUpdateConnection(log);

            //Create schema
            await CreateSchema(log);

            //Ingest content
            await IngestContent(log);

            //Return OK
            return new OkObjectResult("All steps are completed");

        }



        public async Task CreateOrUpdateConnection(ILogger log)
        {

            _ = _graphAPIService ?? throw new MemberAccessException("graphAPIService is null");

            try
            {
                ExternalConnectionCollectionResponse existingConnections = await _graphAPIService.GetExistingConnectionsAsync();
                ExternalConnection connection = existingConnections.Value.FirstOrDefault(x => x.Id == ConnectionConfiguration.ConnectionID);

                if (connection == null)
                {
                    log.LogInformation("No connection was found, creating it");
                    connection = await _graphAPIService.CreateConnectionAsync();
                    log.LogInformation("Done");
                }
                else
                {
                    log.LogInformation("Connection already exists, Skipping");
                    // log.LogInformation("Connection already exists, updating it");
                    // await _graphAPIService.UpdateConnectionAsync(ConnectionConfiguration.ConnectionID);
                    // log.LogInformation("Updated");
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
            _ = _graphAPIService ?? throw new MemberAccessException("graphAPIService is null");

            try
            {
                //Get Schema    
                log.LogInformation("Getting Schema");
                Schema schema = await _graphAPIService.GetSchemaAsync(ConnectionConfiguration.ConnectionID);
                log.LogInformation("Got Schema");
                //Schema is null then provision schema  
                if (schema == null)
                {
                    log.LogInformation("Provisioning Schema, this will take a few minutes");
                    await _graphAPIService.RegisterSchemaAsync(ConnectionConfiguration.ConnectionID, ConnectionConfiguration.Schema);
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


            //transform modules to external items
            log.LogInformation("Transforming modules to external items...");
            var transformedModules = GraphHelper.TransformModulesToExternalItems(modules);
            log.LogInformation("Transformed");

            foreach (var item in transformedModules)
            {
                log.LogInformation(string.Format("Loading item {0}...", item.Id));
                log.LogInformation($"--------------------------------------------------");
                try
                {
                    await _graphAPIService.AddOrUpdateItemAsync(ConnectionConfiguration.ConnectionID, item);
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
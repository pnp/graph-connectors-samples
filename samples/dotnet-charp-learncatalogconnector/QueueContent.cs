using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models.ExternalConnectors;
using Newtonsoft.Json;
using O365C.GraphConnector.MicrosoftLearn.Helpers;
using O365C.GraphConnector.MicrosoftLearn.Models;
using O365C.GraphConnector.MicrosoftLearn.Services;
using O365C.GraphConnector.MicrosoftLearn.Util;

namespace O365C.GraphConnector.MicrosoftLearn
{
    public class QueueContent
    {

        private readonly IGraphAPIService _graphAPIService;
        private readonly ICatalogApiService _catalogApiService;
        private readonly ILogger _logger;

        public QueueContent(
            IGraphAPIService graphAPIService,
            ILoggerFactory loggerFactory, ICatalogApiService catalogApiService)
        {
            _graphAPIService = graphAPIService;
            _logger = loggerFactory.CreateLogger<QueueContent>();
            _catalogApiService = catalogApiService;
        }

        [FunctionName("QueueContent")]
        public async Task RunAsync([QueueTrigger("queue-content", Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"QueueContent Queue trigger function executed at: {DateTime.Now}");

            log.LogInformation($"QueueItem: {myQueueItem}");

            CommonConstants.ResultLayoutFilePath = Path.Combine(context.FunctionAppDirectory, "Cards", "resultLayout.json");

            //Get the myQueueItem and deserialize it to ConnectionMessage object
            ConnectionMessage connectionMessage = JsonConvert.DeserializeObject<ConnectionMessage>(myQueueItem);


            if (connectionMessage.Action == null)
            {
                log.LogError("Action is null");
                throw new ArgumentNullException("Action is null");
            }
            else if (connectionMessage.Action == "create")
            {
                log.LogInformation("Action is create");
                //Create connection if does not exist

                log.LogInformation("Creating connection....");
                await CreateConnection(log);
                log.LogInformation("Created");

                //Create schema if does not exist
                log.LogInformation("Creating schema....");
                await CreateSchema(log);
                log.LogInformation("Created");

                //Ingest content
                log.LogInformation("Ingesting content....");
                await IngestContent(log);
                log.LogInformation("Ingested");

            }
            else if (connectionMessage.Action == "ingest")
            {
                log.LogInformation("Action is ingest");
                //Ingest content
                await IngestContent(log);

            }
            else if (connectionMessage.Action == "delete")
            {
                log.LogInformation("Action is delete");
                log.LogInformation("Deleting connection....");

                try
                {
                    //await _graphAPIService.DeleteConnectionAsync(ConnectionConfiguration.ConnectionID);
                    log.LogInformation("Deleted");
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "Error deleting connection");
                    throw;
                }

            }
            else
            {
                log.LogError("Action is not valid");
                throw new ArgumentException("Action is not valid");
            }

            log.LogInformation("All steps are completed");           
           
        }

        public async Task CreateConnection(ILogger log)
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


            //get existing connection by id
            log.LogInformation("Getting existing connection...");
            ExternalConnection connection = await _graphAPIService.GetConnectionAsync(ConnectionConfiguration.ConnectionID);

            //if connection is not null then do the following else throw exception
            if (connection == null)
            {
                log.LogError("Connection is null");
                throw new ArgumentNullException("Connection is null");
            }

            //Get schema and check if it is null then throw an exception
            log.LogInformation("Getting schema...");
            Schema schema = await _graphAPIService.GetSchemaAsync(ConnectionConfiguration.ConnectionID);
            if (schema == null)
            {
                log.LogError("Schema is null");
                throw new ArgumentNullException("Schema is null");
            }


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
                    //throw an exception
                    throw;

                }
            }




        }


    }
}

using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models.ExternalConnectors;
using Newtonsoft.Json;
using O365C.GraphConnector.MicrosoftLearn.Services;
using O365C.GraphConnector.MicrosoftLearn.Util;

namespace O365C.GraphConnector.MicrosoftLearn
{
    public class QueueConnection
    {

        
        private readonly ILogger _logger;

        private readonly IGraphHttpService _graphHttpService;
        private readonly ICatalogApiService _catalogApiService;

        public QueueConnection(
            ILoggerFactory loggerFactory,
            IGraphHttpService graphHttpService,
            ICatalogApiService catalogApiService)
        {
            _logger = loggerFactory.CreateLogger<QueueConnection>();            
            _graphHttpService = graphHttpService;
            _catalogApiService = catalogApiService;
        }

        [FunctionName("QueueConnection")]
        public async Task RunAsync([QueueTrigger("queue-connection", Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log, ExecutionContext context, [Queue("queue-content", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> queue)
        {
            log.LogInformation($"QueueConnection function triggered: {myQueueItem}");

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
                await CreateConnection(log, connectionMessage);
                log.LogInformation("Created");

                //Create schema if does not exist
                log.LogInformation("Creating schema....");
                await CreateSchema(log);
                log.LogInformation("Created");

                //Ingest content                 
                 
                //Put the message to the queue-content
                connectionMessage.Action = "ingest";
                string queueMessage = JsonConvert.SerializeObject(connectionMessage);
                await queue.AddAsync(queueMessage);
            
                

            }
            else if (connectionMessage.Action == "delete")
            {
                log.LogInformation("Action is delete");
                log.LogInformation("Deleting connection....");

                try
                {
                    await _graphHttpService.DeleteConnectionAsync(ConnectionConfiguration.ConnectionID);
                    log.LogInformation("Deleted");
                }
                catch (Exception ex)
                {
                    log.LogError(ex, "Error deleting connection");
                    throw;
                }

            }
            log.LogInformation("All steps are completed");

        }

        private async Task CreateConnection(ILogger log, ConnectionMessage connectionMessage)
        {
            _ = _graphHttpService ?? throw new MemberAccessException("graphHttpService is null");

            try
            {
                ExternalConnection connection  = await _graphHttpService.GetConnectionAsync(ConnectionConfiguration.ConnectionID);                

                if (connection == null)
                {
                    log.LogInformation("No connection was found, creating it...");
                    connection = await _graphHttpService.CreateConnectionAsync(connectionMessage.ConnectorId, connectionMessage.ConnectorTicket);
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

        private async Task CreateSchema(ILogger log)
        {
            _ = _graphHttpService ?? throw new MemberAccessException("_graphHttpService is null");

            try
            {
                //Get Schema    
                log.LogInformation("Getting Schema");
                Schema schema = await _graphHttpService.GetSchemaAsync(ConnectionConfiguration.ConnectionID);
               
                //Schema is null then provision schema  
                if (schema == null)
                {                    
                    log.LogInformation("Schema not found: Provisioning Schema, this will take a few minutes");
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

       

    }
}


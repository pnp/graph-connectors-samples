using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

        private readonly IGraphHttpService _graphHttpService;

        public QueueContent(
            IGraphAPIService graphAPIService,
            ILoggerFactory loggerFactory, ICatalogApiService catalogApiService, IGraphHttpService graphHttpService)
        {
            _graphAPIService = graphAPIService;
            _logger = loggerFactory.CreateLogger<QueueContent>();
            _catalogApiService = catalogApiService;
            _graphHttpService = graphHttpService;
        }

        [FunctionName("QueueContent")]
        public async Task RunAsync([QueueTrigger("queue-content", Connection = "AzureWebJobsStorage")] string myQueueItem, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"QueueContent Queue trigger function executed at: {DateTime.Now}");

            log.LogInformation($"QueueItem: {myQueueItem}");

            CommonConstants.ResultLayoutFilePath = Path.Combine(context.FunctionAppDirectory, "Assets", "resultLayout.json");

            //Get the myQueueItem and deserialize it to ConnectionMessage object
            ConnectionMessage connectionMessage = JsonConvert.DeserializeObject<ConnectionMessage>(myQueueItem);


            if (connectionMessage.Action == null)
            {
                log.LogError("Action is null");
                throw new ArgumentNullException("Action is null");
            }          
            else if (connectionMessage.Action == "ingest")
            {
                log.LogInformation("Action is ingest");
                //Ingest content
                await IngestContent(log);
            }
           
            else
            {
                log.LogError("Action is not valid");
                throw new ArgumentException("Action is not valid");
            }

            log.LogInformation("All steps are completed");           
           
        }

   


      private async Task IngestContent(ILogger log)
        {
             _ = _graphHttpService ?? throw new MemberAccessException("graphHttpService is null");
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

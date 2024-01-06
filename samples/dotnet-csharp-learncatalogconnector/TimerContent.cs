using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace O365C.GraphConnector.MicrosoftLearn
{
    public class TimerContent
    {
        [FunctionName("TimerContent")]
        public void Run([TimerTrigger("0 30 6 * * *")] TimerInfo myTimer, ILogger log,
        [Queue("queue-content", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> queue)
        {
            log.LogInformation($"TimerContent Timer trigger function executed at: {DateTime.Now}");

            ConnectionMessage connectionMessage = new ConnectionMessage
            {
                Action = "ingest"
            };

            //deserlize the connectionMessage object to string
            string message = JsonConvert.SerializeObject(connectionMessage);
            //put the message to the queue
            queue.AddAsync(message);
        }
    }
}

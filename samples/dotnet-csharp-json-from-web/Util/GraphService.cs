using Azure.Identity;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphConnectorDanToftBlog.Util {

    class GraphService {

        static GraphServiceClient? _client;

        public static GraphServiceClient Client {
            get {
                if (_client is null) {
                    var credential = new ClientSecretCredential(Configuration.TenantId, Configuration.ClientId, Configuration.ClientSecret);
                    var handlers = GraphClientFactory.CreateDefaultHandlers();
                    handlers.Insert(0, new CompleteJobWithDelayHandler(60000));
    
                    var httpClient = GraphClientFactory.Create(handlers);

                    _client = new GraphServiceClient(httpClient, credential);
                }

                return _client;
            }
        }
    }
}
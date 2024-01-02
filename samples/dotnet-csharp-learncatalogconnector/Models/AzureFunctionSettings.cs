using System.Security.Cryptography.X509Certificates;

namespace O365C.GraphConnector.MicrosoftLearn.Models
{ 
    public class AzureFunctionSettings
    {        
        public string TenantId { get; set; }        
        public string ClientId { get; set; }            
        public string ClientSecret { get; set; }        
    }
}

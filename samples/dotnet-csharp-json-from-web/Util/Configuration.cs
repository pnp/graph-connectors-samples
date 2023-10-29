using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphConnectorDanToftBlog.Util {
    internal class Configuration {

        public static string ClientId { get { return Environment.GetEnvironmentVariable("ClientId"); } }
        public static string TenantId { get { return Environment.GetEnvironmentVariable("TenantId"); } }
        public static string ClientSecret { get { return Environment.GetEnvironmentVariable("ClientSecret"); } }
        public static string Base_Url { get { return "https://blog.dan-toft.dk"; } }
    }
}

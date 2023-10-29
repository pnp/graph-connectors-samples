using GraphConnectorDanToftBlog.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphConnectorDanToftBlog.Models {

    internal class Blogpost {
        public string Id { get { return permalink.Replace("/", ""); } }
        public string content { get; set; }
        public string date { get; set; }
        public DateTime dateParsed { get { return DateTime.Parse(date); } }
        public string permalink { get; set; }
        public string fulllink { get { return $"{Configuration.Base_Url}{permalink}"; } }
        public string section { get; set; }
        public string title { get; set; }
    }
}

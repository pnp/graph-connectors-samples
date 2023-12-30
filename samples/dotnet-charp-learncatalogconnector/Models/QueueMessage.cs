using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using O365C.GraphConnector.MicrosoftLearn.Models;

namespace O365C.GraphConnector.MicrosoftLearn.Model
{

    public enum NotificationType
    {
        Http,
        Timer,
        Adhoc
    }

    public class Notification
    {
        public NotificationType Type { get; set; }
        public string Url { get; set; }
        public string Schedule { get; set; }
        public string Content { get; set; }
    }
}
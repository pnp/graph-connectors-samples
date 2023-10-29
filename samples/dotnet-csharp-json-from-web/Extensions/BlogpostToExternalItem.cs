using GraphConnectorDanToftBlog.Models;
using Microsoft.Graph.Models.ExternalConnectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphConnectorDanToftBlog.Extensions {
    internal static class BlogpostToExternalItem {
        private static string MyUserId = "1d29bd99-af71-447b-9f44-ce8e97ce28d3";
        public static ExternalItem ToExternalItem(this Blogpost post) {
            return new ExternalItem() {
                Id = post.Id,
                Properties = new() {
                    AdditionalData = new Dictionary<string, object> {
                        { "title", post.title },
                        { "url", post.fulllink },
                        { "date", post.dateParsed },
                    }
                },
                Content = new() {
                    Value = System.Web.HttpUtility.HtmlDecode(post.content),
                    Type = ExternalItemContentType.Text
                },
                Acl = new() {
                    new() {
                        Type = AclType.Everyone,
                        Value = "everyone",
                        AccessType = AccessType.Grant
                    }
                },
                Activities = new() {
                    new() {
                        OdataType = "#microsoft.graph.externalConnectors.externalActivity",
                        Type = ExternalActivityType.Created,
                        StartDateTime = post.dateParsed,
                        PerformedBy = new Identity {
                            Type = IdentityType.User,
                            Id = MyUserId
                        }
                    }
                }
            };
        }
    }
}

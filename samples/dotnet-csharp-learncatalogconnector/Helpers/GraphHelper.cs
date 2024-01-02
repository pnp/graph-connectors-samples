using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models.ExternalConnectors;
using O365C.GraphConnector.MicrosoftLearn.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace O365C.GraphConnector.MicrosoftLearn.Helpers
{
    public static class GraphHelper
    {
        private const string AuthorityHost = "https://login.microsoftonline.com";

        //public static GraphServiceClient InitializeGraphClient(string tenantId, string clientId, string clientSecret)
        //{
        //    //var scope = new string[] { "User.Read" };

        //    var options = new ClientSecretCredentialOptions
        //    {
        //        AuthorityHost = new Uri(AuthorityHost),
        //    };

        //    var clientCredential = new ClientSecretCredential(clientId, tenantId, clientSecret, options);

        //    try
        //    {
        //        //var graphClient = new GraphServiceClient(clientCredential, scope);
        //        var graphClient = new GraphServiceClient(clientCredential);
        //        return graphClient;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public static IEnumerable<ExternalItem> TransformModulesToExternalItems(IEnumerable<Module> content)
        {

            return content.Select(module =>
            {
                return new ExternalItem
                {
                    Id = Regex.Replace(module.Uid, "[^a-zA-Z0-9-]", ""),
                    Acl = new()
                    {
                            new()
                            {
                                Type = AclType.Everyone,
                                Value = "everyone",
                                AccessType = AccessType.Grant,
                            },
                    },
                    Properties = new()
                    {
                        AdditionalData = new Dictionary<string, object> {

                                { "Summary", module.Summary ?? "" },
                                { "Levels", string.Join(",", module.Levels ?? new List<string>()) },
                                { "Roles", string.Join(",", module.Roles ?? new List<string>()) },
                                { "Products", string.Join(",", module.Products ?? new List<string>()) },
                                { "Subjects", string.Join(",", module.Subjects ?? new List<string>()) },
                                { "Uid", module.Uid ?? "" },
                                { "Title", module.Title ?? "" },
                                { "Duration", module.Duration.ToString() },
                                { "Rating", module.Rating != null ? GetStarRating((int)module.Rating.Average) : "" },
                                { "IconUrl", module.IconUrl ?? "" },
                                { "SocialImageUrl", module.SocialImageUrl ?? "" },
                                { "LastModified", module.LastModified },
                                { "Url", module.Url ?? "" },
                                { "Units", string.Join(",", module.Units ?? new List<string>()) },
                                { "NumberOfUnits", module.NumberOfUnits.ToString() }

                        }
                    },
                    Activities = new()
            {
               new()
               {
                   OdataType = "#microsoft.graph.externalConnectors.externalActivity",
                   Type = ExternalActivityType.Created,
                   StartDateTime = new DateTime(),
                   PerformedBy = new Identity
                   {
                         Type = IdentityType.User,
                         Id = "2a5de346-1d63-4c7a-897f-b1f4b5316fe5"
                   }

               },
            },

                    Content = new()
                    {
                        Value = module.Summary,
                        Type = ExternalItemContentType.Text
                    },
                };
            });
        }

        private static object GetStarRating(int count)
        {
            switch (count)
            {
                case 0:
                    return "";
                case 1:
                    return "⭐️";
                case 2:
                    return "⭐️⭐️";
                case 3:
                    return "⭐️⭐️⭐️";
                case 4:
                    return "⭐️⭐️⭐️⭐️ ";
                case 5:
                    return "⭐️⭐️⭐️⭐️⭐️";
                default:
                    return "";
            }
        }
    }
}

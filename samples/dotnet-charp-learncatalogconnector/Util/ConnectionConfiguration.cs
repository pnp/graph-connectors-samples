using Microsoft.Graph.Models.ExternalConnectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using Microsoft.Graph.Models;

namespace O365C.GraphConnector.MicrosoftLearn.Util
{
    internal class ConnectionConfiguration
    {

        private static Dictionary<string, object> _layout;
        public static Dictionary<string, object> Layout
        {
            get
            {
                if (_layout is null)
                {
                    var adaptiveCard = File.ReadAllText(CommonConstants.ResultLayoutFilePath);
                    _layout = JsonSerializer.Deserialize<Dictionary<string, object>>(adaptiveCard);
                }

                return _layout!;
            }
        }

        public static string ConnectionID = "LearnConnector";

        public static ExternalConnection ExternalConnection
        {
            get
            {
                return new ExternalConnection
                {
                    Id = ConnectionID,
                    Name = "Microsoft Learn Catalog API",
                    Description = "The Microsoft Learn Catalog API is a REST-based Web API lets you send a web-based query to Microsoft Learn and get back details about the available training content and certification exams such as titles, products covered, levels, links to training, and other metadata and returns a JSON-encoded response with the information.",
                    ActivitySettings = new ActivitySettings()
                    {
                        UrlToItemResolvers = new() {
                            new ItemIdResolver {
                                UrlMatchInfo = new() {
                                    BaseUrls = new() { CommonConstants.BaseUrl },
                                    UrlPattern = "/training/modules/(?<slug>[^/]+)/?",
                                },
                                ItemId = "{slug}",
                                Priority = 1,

                            }
                        },
                    },
                    SearchSettings = new()
                    {
                        SearchResultTemplates = new() {
                           new() {
                               Id = "LearnAPISrc",
                               Priority = 1,
                               Layout = new Json {
                                    AdditionalData = Layout
                               }
                           }
                       }
                    }                    
                };
            }
        }

        public static Schema Schema
        {
            get
            {
                return new Schema()
                {
                    BaseType = "microsoft.graph.externalItem",
                    Properties = new List<Property>() {
                        new Property {
                            Name = "Summary",
                            Type = PropertyType.String,
                            IsQueryable = true,
                            IsSearchable = true,
                            IsRetrievable = true,
                        },
                        new Property {
                            Name = "Levels",
                            Type = PropertyType.String,
                            IsRetrievable = true
                        },
                        new Property {
                            Name = "Roles",
                            Type = PropertyType.String,
                            IsRetrievable = true
                        },
                        new Property {
                            Name = "Products",
                            Type = PropertyType.String,
                            IsRetrievable = true
                        },
                        new Property {
                            Name = "Subjects",
                            Type = PropertyType.String,
                            IsRetrievable = true
                        },
                        new Property {
                            Name = "Uid",
                            Type = PropertyType.String,
                            IsRetrievable = true
                        },
                        new Property {
                            Name = "Title",
                            Type = PropertyType.String,
                            IsQueryable = true,
                            IsSearchable = true,
                            IsRetrievable = true,
                             Labels = new() { Label.Title }
                        },
                         new Property {
                            Name = "Duration",
                            Type = PropertyType.String,
                            IsRetrievable = true
                        },
                          new Property {
                            Name = "Rating",
                            Type = PropertyType.String,
                            IsRetrievable = true
                        },
                        new Property {
                            Name = "IconUrl",
                            Type = PropertyType.String,
                            IsRetrievable = true
                        },
                        new Property {
                            Name = "SocialImageUrl",
                            Type = PropertyType.String,
                            IsRetrievable = true
                        },
                        new Property {
                            Name = "LastModified",
                            Type = PropertyType.DateTime,
                            IsQueryable = true,
                            IsRefinable = true,
                            IsRetrievable = true,
                            Labels = new() { Label.LastModifiedDateTime }
                        },
                         new Property {
                            Name = "Url",
                            Type = PropertyType.String,
                            IsRetrievable = true,
                            Labels = new() { Label.Url }
                        },
                        new Property {
                            Name = "Units",
                            Type = PropertyType.String,
                            IsRetrievable = true
                        },
                        new Property {
                            Name = "NumberOfUnits",
                            Type = PropertyType.String,
                            IsRetrievable = true,
                        },
                    }
                };
            }
        }
    }
}


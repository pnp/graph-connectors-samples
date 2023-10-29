using System.Text.Json;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ExternalConnectors;

namespace GraphConnectorDanToftBlog.Util {
    internal class ConnectionConfiguration {

        private static Dictionary<string, object>? _layout;
        private static Dictionary<string, object> Layout {
            get {
                if (_layout is null) {
                    var adaptiveCard = File.ReadAllText("resultLayout.json");
                    _layout = JsonSerializer.Deserialize<Dictionary<string, object>>(adaptiveCard);
                }

                return _layout!;
            }
        }

        public static string ConnectionID = "BlogDanToftDk";
        public static ExternalConnection ExternalConnection {
            get {
                return new ExternalConnection {
                    Id = ConnectionID,
                    Name = "M365 TL;DR - Dan Toft",
                    Description = "A blog by Dan Toft, a M365 enthusiast!",
                    ActivitySettings = new ActivitySettings() {
                        UrlToItemResolvers = new() {
                            new ItemIdResolver {
                                UrlMatchInfo = new() {
                                    BaseUrls = new() { Configuration.Base_Url },
                                    UrlPattern = "/(?<year>[^/]+)/(?<month>[^/]+)/(?<slug>[^/]+)/?",
                                },
                                ItemId = "{year}{month}{slug}",
                                Priority = 1,
                                
                            }
                        },
                    },
                    SearchSettings = new() {
                        SearchResultTemplates = new() {
                            new() {
                                Id = ("AC1"+ ConnectionID).Substring(0,16),
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

        public static Schema Schema {
            get {
                return new Schema() {
                    BaseType = "microsoft.graph.externalItem",
                    Properties = new List<Property>() {
                        new Property {
                            Name = "title",
                            Type = PropertyType.String,
                            IsQueryable = true,
                            IsSearchable = true,
                            IsRetrievable = true,
                            Labels = new() { Label.Title }
                        },
                        new Property {
                            Name = "url",
                            Type = PropertyType.String,
                            IsRetrievable = true,
                            Labels = new() { Label.Url }
                        },
                        new Property {
                            Name = "date",
                            Type = PropertyType.DateTime,
                            IsQueryable = true,
                            IsRefinable = true,
                            IsRetrievable = true,
                            Labels = new() { Label.LastModifiedDateTime }
                        }
                    }
                };
            }
        }
    }
}

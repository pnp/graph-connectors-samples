using System.Text.Json;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ExternalConnectors;

namespace GraphDocsConnector
{
    static class ConnectionConfiguration
    {
        private static Dictionary<string, object>? _layout;
        private static Dictionary<string, object> Layout
        {
            get
            {
                if (_layout is null)
                {
                    var adaptiveCard = File.ReadAllText("resultLayout.json");
                    _layout = JsonSerializer.Deserialize<Dictionary<string, object>>(adaptiveCard);
                }
                return _layout!;
            }
        }

        public static ExternalConnection ExternalConnection => new ExternalConnection
        {
            Id = "msgraphdocs",
            Name = "Microsoft Graph documentation",
            Description = "Documentation for Microsoft Graph API which explains what Microsoft Graph is and how to use it.",
            ActivitySettings = new()
            {
                UrlToItemResolvers = new()
                {
                    new ItemIdResolver
                    {
                        UrlMatchInfo = new()
                        {
                            BaseUrls = new() { "https://learn.microsoft.com" },
                            UrlPattern = "/[^/]+/graph/auth/(?<slug>[^/]+)",
                        },
                        ItemId = "auth__{slug}",
                        Priority = 1
                    },
                    new ItemIdResolver
                    {
                        UrlMatchInfo = new()
                        {
                            BaseUrls = new() { "https://learn.microsoft.com" },
                            UrlPattern = "/[^/]+/graph/sdks/(?<slug>[^/]+)",
                        },
                        ItemId = "sdks__{slug}",
                        Priority = 2
                    },
                    new ItemIdResolver
                    {
                        UrlMatchInfo = new()
                        {
                            BaseUrls = new() { "https://learn.microsoft.com" },
                            UrlPattern = "/[^/]+/graph/(?<slug>[^/]+)",
                        },
                        ItemId = "{slug}",
                        Priority = 3
                    }
                }
            },
            SearchSettings = new()
            {
                SearchResultTemplates = new()
                {
                    new()
                    {
                        Id = "msgraphdocs",
                        Priority = 1,
                        Layout = new Json
                        {
                            AdditionalData = Layout
                        }
                    }
                }
            }
        };

        public static Schema Schema => new Schema
        {
            BaseType = "microsoft.graph.externalItem",
            Properties = new()
            {
                new Property
                {
                    Name = "title",
                    Type = PropertyType.String,
                    IsQueryable = true,
                    IsSearchable = true,
                    IsRetrievable = true,
                    Labels = new() { Label.Title }
                },
                new Property
                {
                    Name = "description",
                    Type = PropertyType.String,
                    IsQueryable = true,
                    IsSearchable = true,
                    IsRetrievable = true
                },
                new Property
                {
                    Name = "url",
                    Type = PropertyType.String,
                    IsRetrievable = true,
                    Labels = new() { Label.Url }
                }
            }
        };
    }
}

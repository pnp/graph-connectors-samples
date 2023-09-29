using Microsoft.Graph.Models.ExternalConnectors;

static class ConnectionConfiguration
{
  public static ExternalConnection ExternalConnection
  {
    get
    {
      return new ExternalConnection
      {
        Id = "waldekblogdotnet",
        Name = "Waldek Mastykarz (blog); .NET",
        Description = "Tips and best practices for building applications on Microsoft 365 by Waldek Mastykarz - Microsoft 365 Cloud Developer Advocate",
        ActivitySettings = new()
        {
          UrlToItemResolvers = new() {
            new ItemIdResolver {
              UrlMatchInfo = new()
              {
                BaseUrls = new() { "https://blog.mastykarz.nl" },
                UrlPattern = "/(?<slug>[^/]+)",
              },
              ItemId = "{slug}",
              Priority = 1
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
      return new Schema
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
            Name = "excerpt",
            Type = PropertyType.String,
            IsQueryable = true,
            IsSearchable = true,
            IsRetrievable = true
          },
          new Property
          {
            Name = "imageUrl",
            Type = PropertyType.String,
            IsRetrievable = true
          },
          new Property
          {
            Name = "url",
            Type = PropertyType.String,
            IsRetrievable = true,
            Labels = new() { Label.Url }
          },
          new Property
          {
            Name = "date",
            Type = PropertyType.DateTime,
            IsQueryable = true,
            IsRefinable = true,
            IsRetrievable = true,
            Labels = new() { Label.LastModifiedDateTime }
          },
          new Property
          {
            Name = "tags",
            Type = PropertyType.StringCollection,
            IsQueryable = true,
            IsRetrievable = true,
            IsRefinable = true
          }
        }
      };
    }
  }
}
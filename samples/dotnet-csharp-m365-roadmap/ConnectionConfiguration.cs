using Microsoft.Graph.Models.ExternalConnectors;
static class ConnectionConfiguration
{
  public static ExternalConnection ExternalConnection
  {
    get
    {
      return new ExternalConnection
      {
        Id = "roadmapmicrosoft365",
        Name = "Microsoft 365 Roadmap",
        Description = "Microsoft 365 Roadmap provides features and latest updates on Microsoft 365 productivity apps and intelligent cloud services.It provides estimated release dates and descriptions for Microsoft 365 features."
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
            Name = "description",
            Type = PropertyType.String,
            IsQueryable = true,
            IsSearchable = true,
            IsRetrievable = true
          },
          new Property
          {
            Name = "iconUrl",
            Type = PropertyType.String,
            IsRetrievable = true,
            Labels = new() { Label.IconUrl }
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
}
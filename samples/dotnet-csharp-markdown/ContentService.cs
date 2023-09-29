using Markdig;
using Microsoft.Graph.Models.ExternalConnectors;
using YamlDotNet.Serialization;

public interface IMarkdown
{
  string? Markdown { get; set; }
}

class BlogPost : IMarkdown
{
  [YamlMember(Alias = "title")]
  public string? Title { get; set; }
  [YamlMember(Alias = "slug")]
  public string? Slug { get; set; }
  [YamlMember(Alias = "excerpt")]
  public string? Excerpt { get; set; }
  [YamlMember(Alias = "image")]
  public string? Image { get; set; }
  [YamlMember(Alias = "date")]
  public DateTime? Date { get; set; }
  [YamlMember(Alias = "tags")]
  public IList<string>? Tags { get; set; }
  public string? Markdown { get; set; }
  public string? Content { get; set; }
}

static class ContentService
{
  static IEnumerable<BlogPost> Extract()
  {
    var blogPosts = new List<BlogPost>();

    var contentFolder = "content";
    var files = Directory.GetFiles(contentFolder);

    foreach (var file in files)
    {
      try
      {
        var contents = File.ReadAllText(file);
        var blogPost = contents.GetContents<BlogPost>();
        blogPost.Content = Markdown.ToPlainText(blogPost.Markdown ?? "");
        blogPosts.Add(blogPost);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    return blogPosts;
  }

  static IEnumerable<ExternalItem> Transform(IEnumerable<BlogPost> content)
  {
    var baseUrl = new Uri("https://blog.mastykarz.nl");

    return content.Select(a =>
    {
      return new ExternalItem
      {
        Id = a.Slug,
        Properties = new()
        {
          AdditionalData = new Dictionary<string, object> {
            { "title", a.Title ?? "" },
            { "excerpt", a.Excerpt ?? "" },
            { "imageUrl", new Uri(baseUrl, a.Image).ToString() },
            { "url", new Uri(baseUrl, a.Slug).ToString() },
            { "date", a.Date ?? DateTime.MinValue },
            { "tags@odata.type", "Collection(String)" },
            { "tags", a.Tags ?? new List<string>() }
          }
        },
        Content = new()
        {
          Value = a.Content ?? "",
          Type = ExternalItemContentType.Text
        },
        Acl = new()
        {
          new()
          {
            Type = AclType.Everyone,
            Value = "everyone",
            AccessType = AccessType.Grant
          }
        },
        Activities = new()
        {
          new()
          {
            OdataType = "#microsoft.graph.externalConnectors.externalActivity",
            Type = ExternalActivityType.Created,
            StartDateTime = a.Date,
            PerformedBy = new Identity
            {
              Type = IdentityType.User,
              Id = "9da37739-ad63-42aa-b0c2-06f7b43e3e9e"
            }
          }
        }
      };
    });
  }

  static async Task Load(IEnumerable<ExternalItem> items)
  {
    foreach (var item in items)
    {
      Console.Write(string.Format("Loading item {0}...", item.Id));
      try
      {
        await GraphService.Client.External
          .Connections[ConnectionConfiguration.ExternalConnection.Id]
          .Items[item.Id]
          .PutAsync(item);
        Console.WriteLine("DONE");
      }
      catch (Exception ex)
      {
        Console.WriteLine("ERROR");
        Console.WriteLine(ex.Message);
      }
    }
  }

  public static async Task LoadContent()
  {
    var content = Extract();
    var transformed = Transform(content);
    await Load(transformed);
  }
}
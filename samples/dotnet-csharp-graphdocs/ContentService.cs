using Markdig;
using Microsoft.Graph.Models.ExternalConnectors;
using YamlDotNet.Serialization;

public interface IMarkdown
{
  string? Markdown { get; set; }
}

class DocsArticle : IMarkdown
{
  [YamlMember(Alias = "title")]
  public string? Title { get; set; }
  [YamlMember(Alias = "description")]
  public string? Description { get; set; }
  public string? Markdown { get; set; }
  public string? Content { get; set; }
  public string? RelativePath { get; set; }
}

static class ContentService
{
  static IEnumerable<DocsArticle> Extract()
  {
    var docs = new List<DocsArticle>();

    var contentFolder = "content";
    var contentFolderPath = Path.Combine(Directory.GetCurrentDirectory(), contentFolder);
    var files = Directory.GetFiles(contentFolder, "*.md", SearchOption.AllDirectories);

    foreach (var file in files)
    {
      try
      {
        var contents = File.ReadAllText(file);
        var doc = contents.GetContents<DocsArticle>();
        doc.Content = Markdown.ToHtml(doc.Markdown ?? "");
        doc.RelativePath = Path.GetRelativePath(contentFolderPath, file);
        docs.Add(doc);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    return docs;
  }

  static string GetDocId(string relativePath)
  {
    var id = relativePath.Replace(Path.DirectorySeparatorChar.ToString(), "__").Replace(".md", "");
    return id;
  }

  static IEnumerable<ExternalItem> Transform(IEnumerable<DocsArticle> content)
  {
    var baseUrl = new Uri("https://learn.microsoft.com/graph/");

    return content.Select(a =>
    {
      var docId = GetDocId(a.RelativePath ?? "");
      return new ExternalItem
      {
        Id = docId,
        Properties = new()
        {
          AdditionalData = new Dictionary<string, object> {
            { "title", a.Title ?? "" },
            { "description", a.Description ?? "" },
            { "url", new Uri(baseUrl, a.RelativePath!.Replace(".md", "")).ToString() }
          }
        },
        Content = new()
        {
          Value = a.Content ?? "",
          Type = ExternalItemContentType.Html
        },
        Acl = new()
        {
          new()
          {
            Type = AclType.Everyone,
            Value = "everyone",
            AccessType = AccessType.Grant
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
          .Connections[Uri.EscapeDataString(ConnectionConfiguration.ExternalConnection.Id!)]
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
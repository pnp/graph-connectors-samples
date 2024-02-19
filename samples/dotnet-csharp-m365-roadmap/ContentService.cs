using Microsoft.Graph.Models.ExternalConnectors;
using System.Xml.Serialization;

[XmlRoot("rss")]
public class RssFeed
{
    [XmlElement("channel")]
    public RssChannel? Channel { get; set; }
}

public class RssChannel
{
    [XmlElement("title")]
    public string? Title { get; set; }
    [XmlElement("link")]
    public string? Link { get; set; }
    [XmlElement("description")]
    public string? Description { get; set; }
    public DateTime LastBuildDate { get; set; }

    [XmlElement("item")]
    public List<RssItem>? Items { get; set; }
}

public class RssItem
{
    [XmlElement("guid")]
    public string? Guid { get; set; }
    [XmlElement("link")]
    public string? Link { get; set; }
    [XmlElement("category")]
    public List<string>? Categories { get; set; }
    [XmlElement("title")]
    public string? Title { get; set; }
    [XmlElement("description")]
    public string? Description { get; set; }
    
    public DateTime PublishedDate { get; set; }

    [XmlElement("updated", Namespace = "http://www.w3.org/2005/Atom")]
    public DateTime? Updated { get; set; }
}

static class ContentService
{

    /// <summary>
    /// Load the Rss Feed channel's items and extract information from them.
    /// </summary>
    /// <returns></returns>
    static async Task<List<RssItem>> Extract()
    {
        var rssFeedRequest = await new HttpClient().GetAsync($"{Configuration.FeedBaseUrl}/RoadmapFeatureRSS");

        string xml = await rssFeedRequest.Content.ReadAsStringAsync();
        var rssItems = new List<RssItem>();

        XmlSerializer serializer = new XmlSerializer(typeof(RssFeed));
        using (TextReader reader = new StringReader(xml))
        {
            RssFeed? feed = (RssFeed?)serializer?.Deserialize(reader);
            if (feed != null && feed.Channel != null && feed.Channel.Items != null)
            {
                foreach (var item in feed.Channel.Items)
                {
                    try
                    {
                        rssItems.Add(item);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
        return rssItems;
    }

    /// <summary>
    /// Converts Rss Feed Items into external items from Microsoft Graph.
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    static IEnumerable<ExternalItem> Transform(List<RssItem> rssListItems)
    {
        return rssListItems.Select(a =>
        {
            return new ExternalItem
            {
                Id = a.Guid,
                Properties = new()
                {
                    AdditionalData = new Dictionary<string, object> {
            { "title", a.Title ?? "" },
            { "description", a.Description ?? "" },
            { "url", a.Link ?? "" },
            { "iconUrl", a.Link ?? "" }
              }
                },
                Content = new()
                {
                    Value = a.Description ?? "",
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
            }
            };
        });
    }

    /// <summary>
    /// load the transformed external items into Microsoft 365.
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
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
        var rssListItems = await Extract();
        var transformed = Transform(rssListItems);
        await Load(transformed);
    }
}
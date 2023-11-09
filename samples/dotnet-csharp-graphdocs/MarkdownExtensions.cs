// from: https://khalidabuhakmeh.com/parse-markdown-front-matter-with-csharp

using Markdig;
using Markdig.Extensions.Yaml;
using Markdig.Syntax;
using YamlDotNet.Serialization;

public static class MarkdownExtensions
{
  private static readonly IDeserializer YamlDeserializer =
      new DeserializerBuilder()
      .IgnoreUnmatchedProperties()
      .Build();

  private static readonly MarkdownPipeline Pipeline
      = new MarkdownPipelineBuilder()
      .UseYamlFrontMatter()
      .Build();

  public static T GetContents<T>(this string markdown) where T: IMarkdown, new()
  {
    var document = Markdown.Parse(markdown, Pipeline);
    var block = document
        .Descendants<YamlFrontMatterBlock>()
        .FirstOrDefault();

    if (block == null)
      return new T { Markdown = markdown };

    var yaml =
        block
        // this is not a mistake
        // we have to call .Lines 2x
        .Lines // StringLineGroup[]
        .Lines // StringLine[]
        .OrderByDescending(x => x.Line)
        .Select(x => $"{x}\n")
        .ToList()
        .Select(x => x.Replace("---", string.Empty))
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Aggregate((s, agg) => agg + s);

    var t = YamlDeserializer.Deserialize<T>(yaml);
    t.Markdown = markdown.Substring(block.Span.End + 1);
    return t;
  }
}
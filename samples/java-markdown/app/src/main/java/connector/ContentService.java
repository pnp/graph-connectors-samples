package connector;

import java.io.FileNotFoundException;
import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.time.LocalDateTime;
import java.time.ZoneOffset;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

import org.commonmark.Extension;
import org.commonmark.ext.front.matter.YamlFrontMatterExtension;
import org.commonmark.ext.front.matter.YamlFrontMatterVisitor;
import org.commonmark.node.Node;
import org.commonmark.parser.Parser;
import org.commonmark.renderer.html.HtmlRenderer;

import com.google.gson.Gson;
import com.google.gson.JsonParser;
import com.microsoft.graph.externalconnectors.models.AccessType;
import com.microsoft.graph.externalconnectors.models.Acl;
import com.microsoft.graph.externalconnectors.models.AclType;
import com.microsoft.graph.externalconnectors.models.ExternalActivity;
import com.microsoft.graph.externalconnectors.models.ExternalActivityType;
import com.microsoft.graph.externalconnectors.models.ExternalItem;
import com.microsoft.graph.externalconnectors.models.ExternalItemContent;
import com.microsoft.graph.externalconnectors.models.ExternalItemContentType;
import com.microsoft.graph.externalconnectors.models.Identity;
import com.microsoft.graph.externalconnectors.models.IdentityType;
import com.microsoft.graph.externalconnectors.models.Properties;
import com.microsoft.graph.externalconnectors.requests.ExternalActivityCollectionPage;

class BlogPost {
    public String url;
    public String content;
    public Map<String, List<String>> metadata;

    private BlogPost() {
    }

    public static Builder builder() {
        return new Builder();
    }

    public static class Builder {
        private BlogPost post;

        private Builder() {
            post = new BlogPost();
        }

        public Builder content(String content) {
            post.content = content;
            return this;
        }

        public Builder metadata(Map<String, List<String>> metadata) {
            post.metadata = metadata;
            return this;
        }

        public BlogPost build() {
            return post;
        }
    }
}

public class ContentService {
    private static List<BlogPost> extract() throws IOException {
        final Path basePath = Paths.get(System.getProperty("user.dir"));
        final Path contentDir = basePath.resolve("../content");
        if (!Files.exists(contentDir)) {
            throw new FileNotFoundException(String.format("Content directory %s does not exist%n", contentDir));
        }

        final List<Path> filePaths = Files.walk(contentDir)
                .filter(Files::isRegularFile)
                .collect(Collectors.toList());

        List<Extension> extensions = Arrays.asList(YamlFrontMatterExtension.create());

        final Parser parser = Parser.builder()
                .extensions(extensions)
                .build();

        final List<BlogPost> blogPosts = new ArrayList<>();

        for (Path filePath : filePaths) {
            final String content = Files.readString(filePath);
            final Node document = parser.parse(content);
            YamlFrontMatterVisitor yamlVisitor = new YamlFrontMatterVisitor();
            document.accept(yamlVisitor);
            final Map<String, List<String>> yaml = yamlVisitor.getData();

            HtmlRenderer renderer = HtmlRenderer.builder().build();
            final String html = renderer.render(document);
            
            final BlogPost post = BlogPost.builder()
                    .content(html)
                    .metadata(yaml)
                    .build();

            blogPosts.add(post);
        }

        return blogPosts;
    }

    private static List<ExternalItem> transform(List<BlogPost> items) throws MalformedURLException {
        final List<ExternalItem> transformed = new ArrayList<>();
        final URL baseUrl = new URL("https://blog.mastykarz.nl");
        final DateTimeFormatter isoFormatter = DateTimeFormatter.ISO_DATE_TIME;
        final DateTimeFormatter localFormatter = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");
        
        for (BlogPost item : items) {
            final Acl acl = new Acl();
            acl.accessType = AccessType.GRANT;
            acl.type = AclType.EVERYONE;
            acl.value = "everyone";

            final ExternalItemContent content = new ExternalItemContent();
            content.value = item.content;
            content.type = ExternalItemContentType.HTML;

            final LocalDateTime dateTime = LocalDateTime.parse(item.metadata.get("date").get(0), localFormatter);
            final Gson gson = new Gson();

            final Properties properties = new Properties();
            properties.additionalDataManager()
                .put("title", JsonParser.parseString(String.format("\"%s\"", item.metadata.get("title").get(0))));
            properties.additionalDataManager()
                .put("excerpt", JsonParser.parseString(String.format("\"%s\"", item.metadata.get("excerpt").get(0))));
            properties.additionalDataManager()
                .put("imageUrl", JsonParser.parseString(String.format("\"%s\"", new URL(baseUrl, item.metadata.get("image").get(0)).toString())));
            properties.additionalDataManager()
                .put("url", JsonParser.parseString(String.format("\"%s\"", new URL(baseUrl, item.metadata.get("slug").get(0)).toString())));
            properties.additionalDataManager()
                .put("date", JsonParser.parseString(String.format("\"%s\"", dateTime.format(isoFormatter).toString())));
            properties.additionalDataManager()
                .put("tags@odata.type", JsonParser.parseString(String.format("\"%s\"", "Collection(String)")));
            properties.additionalDataManager()
                .put("tags", gson.toJsonTree(item.metadata.get("tags")).getAsJsonArray());

            // uncomment and replace with your own user ID
            // to record the activity as performed by you
            //
            // final Identity createdBy = new Identity();
            // createdBy.type = IdentityType.USER;
            // createdBy.id = "9da37739-ad63-42aa-b0c2-06f7b43e3e9e";

            // final ZoneOffset zoneOffset = ZoneOffset.ofHours(2);

            // final ExternalActivity created = new ExternalActivity();
            // created.oDataType = "#microsoft.graph.externalConnectors.externalActivity";
            // created.type = ExternalActivityType.CREATED;
            // created.performedBy = createdBy;
            // created.startDateTime = dateTime.atOffset(zoneOffset);
            // final List<ExternalActivity> activities = Arrays.asList(created);
            
            final ExternalItem transformedItem = new ExternalItem();
            transformedItem.id = item.metadata.get("slug").get(0);
            transformedItem.content = content;
            transformedItem.acl = Arrays.asList(acl);
            transformedItem.properties = properties;
            // transformedItem.activities = new ExternalActivityCollectionPage(activities, null);

            transformed.add(transformedItem);
        }

        return transformed;
    }

    private static void load(List<ExternalItem> items) {
        for (ExternalItem item : items) {
            System.out.printf("Loading item %s...", item.id);
            try {
                GraphService.getClient().external()
                        .connections(ConnectionConfiguration.getExternalConnection().id)
                        .items(item.id)
                        .buildRequest()
                        .putAsync(item)
                        .get();
                System.out.println("DONE");
            } catch (Exception e) {
                System.out.println("ERROR");
                System.out.println(e.getMessage());
            }
        }
    }

    public static void loadContent() {
        try {
            final List<BlogPost> blogPosts = extract();
            final List<ExternalItem> transformed = transform(blogPosts);
            load(transformed);
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
    }
}

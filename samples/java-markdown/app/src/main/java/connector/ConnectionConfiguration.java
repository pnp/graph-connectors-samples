package connector;

import java.io.IOException;
import java.util.Arrays;

import com.google.gson.JsonParser;
import com.microsoft.graph.externalconnectors.models.ActivitySettings;
import com.microsoft.graph.externalconnectors.models.DisplayTemplate;
import com.microsoft.graph.externalconnectors.models.ExternalConnection;
import com.microsoft.graph.externalconnectors.models.ItemIdResolver;
import com.microsoft.graph.externalconnectors.models.Label;
import com.microsoft.graph.externalconnectors.models.Property;
import com.microsoft.graph.externalconnectors.models.PropertyType;
import com.microsoft.graph.externalconnectors.models.Schema;
import com.microsoft.graph.externalconnectors.models.SearchSettings;
import com.microsoft.graph.externalconnectors.models.UrlMatchInfo;

public class ConnectionConfiguration {
    public static ExternalConnection getExternalConnection() throws IOException {
        final UrlMatchInfo urlMatchInfo = new UrlMatchInfo();
        urlMatchInfo.baseUrls = Arrays.asList("https://blog.mastykarz.nl");
        urlMatchInfo.urlPattern = "/(?<slug>[^/]+)";

        final ItemIdResolver resolver = new ItemIdResolver();
        resolver.itemId = "{slug}";
        resolver.priority = 1;
        resolver.urlMatchInfo = urlMatchInfo;
        resolver.oDataType = "#microsoft.graph.externalConnectors.itemIdResolver";

        final ActivitySettings as = new ActivitySettings();
        as.urlToItemResolvers = Arrays.asList(resolver);

        final DisplayTemplate template = new DisplayTemplate();
        template.id = "waldekblogjava";
        template.priority = 1;
        final String adaptiveCard = new String(
                ConnectionConfiguration.class
                        .getResourceAsStream("resultLayout.json")
                        .readAllBytes());
        template.layout = JsonParser.parseString(adaptiveCard);

        final SearchSettings searchSettings = new SearchSettings();
        searchSettings.searchResultTemplates = Arrays.asList(template);

        final ExternalConnection ec = new ExternalConnection();
        ec.id = "waldekblogjava";
        ec.name = "Waldek Mastykarz (blog); Java";
        ec.description = "Tips and best practices for building applications on Microsoft 365 by Waldek Mastykarz - Microsoft 365 Cloud Developer Advocate";
        ec.activitySettings = as;
        ec.searchSettings = searchSettings;

        return ec;
    }

    public static Schema getSchema() {
        final Schema schema = new Schema();
        schema.baseType = "microsoft.graph.externalItem";

        final Property title = new Property();
        title.name = "title";
        title.type = PropertyType.STRING;
        title.isQueryable = true;
        title.isSearchable = true;
        title.isRetrievable = true;
        title.labels = Arrays.asList(Label.TITLE);

        final Property excerpt = new Property();
        excerpt.name = "excerpt";
        excerpt.type = PropertyType.STRING;
        excerpt.isQueryable = true;
        excerpt.isSearchable = true;
        excerpt.isRetrievable = true;

        final Property imageUrl = new Property();
        imageUrl.name = "imageUrl";
        imageUrl.type = PropertyType.STRING;
        imageUrl.isRetrievable = true;

        final Property url = new Property();
        url.name = "url";
        url.type = PropertyType.STRING;
        url.isRetrievable = true;
        url.labels = Arrays.asList(Label.URL);

        final Property date = new Property();
        date.name = "date";
        date.type = PropertyType.DATE_TIME;
        date.isQueryable = true;
        date.isRetrievable = true;
        date.isRefinable = true;
        date.labels = Arrays.asList(Label.LAST_MODIFIED_DATE_TIME);

        final Property tags = new Property();
        tags.name = "tags";
        tags.type = PropertyType.STRING_COLLECTION;
        tags.isQueryable = true;
        tags.isRetrievable = true;
        tags.isRefinable = true;

        schema.properties = Arrays.asList(
                title,
                excerpt,
                imageUrl,
                url,
                date,
                tags);

        return schema;
    }
}

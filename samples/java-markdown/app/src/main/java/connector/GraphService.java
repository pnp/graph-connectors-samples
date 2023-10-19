package connector;

import java.io.IOException;
import java.util.Properties;

import com.azure.identity.ClientSecretCredential;
import com.azure.identity.ClientSecretCredentialBuilder;
import com.microsoft.graph.authentication.TokenCredentialAuthProvider;
import com.microsoft.graph.httpcore.HttpClients;
import com.microsoft.graph.requests.GraphServiceClient;

import okhttp3.OkHttpClient;

public class GraphService {
    private static GraphServiceClient<okhttp3.Request> client;

    public static GraphServiceClient<okhttp3.Request> getClient() throws IOException {
        if (client == null) {
            final Properties properties = new Properties();
            properties.load(GraphService.class.getResourceAsStream("application.properties"));

            final ClientSecretCredential credential = new ClientSecretCredentialBuilder()
                    .clientId(properties.getProperty("app.clientId"))
                    .clientSecret(properties.getProperty("app.clientSecret"))
                    .tenantId(properties.getProperty("app.tenantId"))
                    .build();
            final TokenCredentialAuthProvider authProvider = new TokenCredentialAuthProvider(credential);
            final OkHttpClient okHttpClient = HttpClients.createDefault(authProvider)
                    .newBuilder()
                    .addInterceptor(new CompleteJobWithDelayHandler(60000))
                    // .addInterceptor(new DebugHandler())
                    .callTimeout(25, java.util.concurrent.TimeUnit.MINUTES)
                    .build();

            client = GraphServiceClient.builder()
                    .httpClient(okHttpClient)
                    .buildClient();
        }

        return client;
    }
}

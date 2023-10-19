package connector;

import java.io.IOException;
import java.nio.charset.StandardCharsets;

import okhttp3.Response;
import okio.Buffer;

public class DebugHandler implements okhttp3.Interceptor {

  @Override
  public Response intercept(Chain chain) throws IOException {
    System.out.println("");
    System.out.printf("Request: %s %s%n", chain.request().method(), chain.request().url().toString());
    System.out.println("Request headers:");
    chain.request().headers().toMultimap()
        .forEach((k, v) -> System.out.printf("%s: %s%n", k, String.join(", ", v)));
    if (chain.request().body() != null) {
      System.out.println("Request body:");
      final Buffer buffer = new Buffer();
      chain.request().body().writeTo(buffer);
      System.out.println(buffer.readString(StandardCharsets.UTF_8));
    }

    final Response response = chain.proceed(chain.request());

    System.out.println("");
    System.out.printf("Response: %s%n", response.code());
    System.out.println("Response headers:");
    response.headers().toMultimap()
        .forEach((k, v) -> System.out.printf("%s: %s%n", k, String.join(", ", v)));
    if (response.body() != null) {
      System.out.println("Response body:");
      System.out.println(response.peekBody(Long.MAX_VALUE).string());
    }

    return response;
  }
}

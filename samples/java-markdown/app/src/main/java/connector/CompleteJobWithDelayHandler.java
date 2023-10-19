package connector;

import java.io.IOException;

import com.microsoft.graph.externalconnectors.models.ConnectionOperation;
import com.microsoft.graph.externalconnectors.models.ConnectionOperationStatus;
import com.microsoft.graph.serializer.ISerializer;

import okhttp3.Request;
import okhttp3.Response;

public class CompleteJobWithDelayHandler implements okhttp3.Interceptor {
    private int delayMs;

    public CompleteJobWithDelayHandler(int delayMs) {
        this.delayMs = delayMs;
    }

    @Override
    public Response intercept(Chain chain) throws IOException {
        Response response = chain.proceed(chain.request());

        while (true) {
            final String location = response.header("Location");
            if (location != null) {
                if (location.indexOf("/operations/") < 0) {
                    // not a job URL we should follow
                    break;
                }

                System.out.printf("Location: %s%n", location);
                System.out.printf("Waiting %dms before following location %s...%n", this.delayMs, location);
                try {
                    Thread.sleep(this.delayMs);
                } catch (InterruptedException e) {
                }

                final Request newRequest = chain.request()
                        .newBuilder()
                        .url(location)
                        .method("GET", null)
                        .build();

                response.close();
                response = chain.proceed(newRequest);
                continue;
            }

            if (!response.isSuccessful()) {
                break;
            }

            if (response.request().url().toString().indexOf("/operations/") < 0) {
                // not a job
                break;
            }

            final String body = response.peekBody(Long.MAX_VALUE).string();
            if (body == null) {
                break;
            }

            final ISerializer serializer = GraphService.getClient().getSerializer();
            if (serializer == null) {
                break;
            }

            final ConnectionOperation operation = serializer.deserializeObject(body,
                    ConnectionOperation.class);
            if (operation == null) {
                break;
            }

            if (operation.status == ConnectionOperationStatus.INPROGRESS) {
                System.out.printf("Waiting %dms before retrying %s...%n", this.delayMs,
                        response.request().url().toString());
                try {
                    Thread.sleep(this.delayMs);
                } catch (InterruptedException e) {
                }

                response.close();
                response = chain.proceed(response.request());
            }
            else {
                break;
            }
        }

        return response;
    }

}

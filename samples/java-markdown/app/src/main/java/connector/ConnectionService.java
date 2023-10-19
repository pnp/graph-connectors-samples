package connector;

import java.io.IOException;
import java.util.concurrent.ExecutionException;

public class ConnectionService {
    private static void CreateConnection() throws InterruptedException, ExecutionException, IOException {
        System.out.print("Creating connection...");

        GraphService.getClient().external().connections()
                .buildRequest()
                .postAsync(ConnectionConfiguration.getExternalConnection()).get();

        System.out.println("DONE");
    }

    private static void CreateSchema() throws InterruptedException, ExecutionException, IOException {
        System.out.println("Creating schema...");

        GraphService.getClient().external()
                .connections(ConnectionConfiguration.getExternalConnection().id)
                .schema()
                .buildRequest()
                .patchAsync(ConnectionConfiguration.getSchema()).get();

        System.out.println("DONE");
    }

    public static void provisionConnection() {
        try {
            CreateConnection();
            CreateSchema();
        } catch (Exception e) {
            System.out.println(e.getMessage());
        }
    }
}

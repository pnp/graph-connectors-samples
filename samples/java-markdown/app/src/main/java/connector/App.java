package connector;

import java.io.IOException;

public class App {
    public static void main(String[] args) throws IOException {
        if (args.length == 0) {
            System.out.println("Please provide a command");
            return;
        }

        switch (args[0]) {
            case "create-connection":
                ConnectionService.provisionConnection();
                break;
            case "load-content":
                ContentService.loadContent();
                break;
            default:
                System.out.println("Unknown command");
                break;
        }
    }
}

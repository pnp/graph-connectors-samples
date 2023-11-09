using System.CommandLine;

var createConnectionCommand = new Command("create-connection", "Creates external connection");
createConnectionCommand.SetHandler(ConnectionService.ProvisionConnection);

var loadContentCommand = new Command("load-content", "Loads content into the external connection");
loadContentCommand.SetHandler(ContentService.LoadContent);

var rootCommand = new RootCommand();
rootCommand.AddCommand(createConnectionCommand);
rootCommand.AddCommand(loadContentCommand);

Environment.Exit(await rootCommand.InvokeAsync(args));
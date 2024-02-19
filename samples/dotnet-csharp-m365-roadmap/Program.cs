// See https://aka.ms/new-console-template for more information
using System.CommandLine;


var provisionConnectionCommand = new Command("provision-connection", "Provisions external connection");
provisionConnectionCommand.SetHandler(ConnectionService.ProvisionConnection);

var loadContentCommand = new Command("load-content", "Loads content   into the external connection");
loadContentCommand.SetHandler(ContentService.LoadContent);

var rootCommand = new RootCommand();
rootCommand.AddCommand(provisionConnectionCommand);
rootCommand.AddCommand(loadContentCommand);

Environment.Exit(await rootCommand.InvokeAsync(args));

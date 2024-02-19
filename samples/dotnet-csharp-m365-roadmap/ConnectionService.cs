using Microsoft.Graph.Models.ExternalConnectors;

static class ConnectionService
{
    async static Task CreateConnection()
    {
        Console.Write("Creating connection...");
        await GraphService.Client.External.Connections.PostAsync(ConnectionConfiguration.ExternalConnection);
        Console.WriteLine("DONE");
    }

    async static Task CreateSchema()
    {
        Console.WriteLine("Creating schema...");
        await GraphService.Client.External.Connections[ConnectionConfiguration.ExternalConnection.Id].Schema
          .PatchAsync(ConnectionConfiguration.Schema);

        do
        {
            var externalConnection = await GraphService.Client.External.Connections[ConnectionConfiguration.ExternalConnection.Id]
              .GetAsync();
            Console.Write($"State: {externalConnection?.State.ToString()}");

            if (externalConnection?.State != ConnectionState.Draft)
            {
                Console.WriteLine();
                break;
            }

            Console.WriteLine($". Waiting 60s...");
            await Task.Delay(60_000);
        }
        while (true);

        Console.WriteLine("DONE");
    }

    public static async Task ProvisionConnection()
    {
        try
        {
            await CreateConnection();
            await CreateSchema();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
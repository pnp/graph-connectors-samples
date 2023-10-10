static class ConnectionService
{
  async static Task CreateConnection()
  {
    Console.Write("Creating connection...");

    await GraphService.Client.External.Connections
      .PostAsync(ConnectionConfiguration.ExternalConnection);

    Console.WriteLine("DONE");
  }

  async static Task CreateSchema()
  {
    Console.WriteLine("Creating schema...");

    await GraphService.Client.External
      .Connections[ConnectionConfiguration.ExternalConnection.Id]
      .Schema
      .PatchAsync(ConnectionConfiguration.Schema);

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
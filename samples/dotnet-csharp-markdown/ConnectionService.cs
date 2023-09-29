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
    Console.Write("Creating schema...");

    // workaround because there's no POST on schema
    var requestInfo = GraphService.Client.External
      .Connections[ConnectionConfiguration.ExternalConnection.Id]
      .Schema
      .ToPatchRequestInformation(ConnectionConfiguration.Schema);
    var requestMessage = await GraphService.Client.RequestAdapter
      .ConvertToNativeRequestAsync<HttpRequestMessage>(requestInfo);
    requestMessage!.Method = HttpMethod.Post;

    await GraphService.HttpClient!.SendAsync(requestMessage);

    // In the future, this should be possible:
    // 
    // await GraphClient.Client.External
    //   .Connections[ConnectionConfiguration.ExternalConnection.Id]
    //   .Schema
    //   .PostAsync(ConnectionConfiguration.Schema);

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
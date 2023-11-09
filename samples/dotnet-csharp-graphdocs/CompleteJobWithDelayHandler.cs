using Microsoft.Graph.Models.ExternalConnectors;
using Microsoft.Kiota.Abstractions.Serialization;

class CompleteJobWithDelayHandler : DelegatingHandler
{
  int delayMs;

  public CompleteJobWithDelayHandler(int delayMs = 10000)
  {
    this.delayMs = delayMs;
  }

  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    var response = await base.SendAsync(request, cancellationToken);

    var location = response.Headers.FirstOrDefault(h => h.Key == "Location").Value?.FirstOrDefault();
    if (location is not null)
    {
      if (location.IndexOf("/operations/") < 0)
      {
        // not a job URL we should follow
        return response;
      }

      Console.WriteLine(string.Format("Location: {0}", location));

      // string interpolation causes NullReferenceException on macOS x64
      // for some reason here, so need to use String.Format instead
      Console.WriteLine(string.Format("Waiting {0}ms before following location {1}...", delayMs, location));
      await Task.Delay(delayMs);

      request.RequestUri = new Uri(location);
      request.Method = HttpMethod.Get;
      request.Content = null;

      var cts = new CancellationTokenSource();
      cts.CancelAfter(TimeSpan.FromMinutes(25));
      return await SendAsync(request, cts.Token);
    }

    if (!response.IsSuccessStatusCode)
    {
      Console.WriteLine(string.Format("Response is not successful: {0}", response.StatusCode));
      try
      {
        var errorBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine(errorBody);
      }
      catch { }
      return response;
    }

    if (request.RequestUri?.AbsolutePath.IndexOf("/operations/") < 0)
    {
      // not a job
      return response;
    }

    var body = await response.Content.ReadAsStringAsync();

    // deserialize the response
    using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(body));
    var parseNode = ParseNodeFactoryRegistry.DefaultInstance.GetRootParseNode("application/json", ms);
    var operation = parseNode.GetObjectValue(ConnectionOperation.CreateFromDiscriminatorValue);

    if (operation?.Status == ConnectionOperationStatus.Inprogress)
    {
      // string interpolation causes NullReferenceException on macOS x64
      // for some reason here, so need to use String.Format instead
      Console.WriteLine(string.Format("Waiting {0}ms before retrying {1}...", delayMs, request.RequestUri));
      await Task.Delay(delayMs);
      return await SendAsync(request, cancellationToken);
    }
    else
    {
      return response;
    }
  }
}
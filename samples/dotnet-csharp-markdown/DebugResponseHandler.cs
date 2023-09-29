class DebugResponseHandler : DelegatingHandler
{
  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    var response = await base.SendAsync(request, cancellationToken);

    Console.WriteLine("");
    Console.WriteLine("Response headers:");
    foreach (var header in response.Headers)
    {
      Console.WriteLine(string.Format("{0}: {1}", header.Key, string.Join(',', header.Value)));
    }
    if (response.Content is not null)
    {
      Console.WriteLine("");
      Console.WriteLine("Response body:");
      var body = await response.Content.ReadAsStringAsync();
      Console.WriteLine(body);
    }

    return response;
  }
}
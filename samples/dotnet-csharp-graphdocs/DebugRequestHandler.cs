class DebugRequestHandler : DelegatingHandler
{
  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    Console.WriteLine("");
    Console.WriteLine(string.Format("Request: {0} {1}", request.Method, request.RequestUri));
    Console.WriteLine("Request headers:");
    foreach (var header in request.Headers)
    {
      Console.WriteLine(string.Format("{0}: {1}", header.Key, string.Join(',', header.Value)));
    }
    if (request.Content is not null)
    {
      Console.WriteLine("");
      Console.WriteLine("Request body:");
      var body = await request.Content.ReadAsStringAsync();
      Console.WriteLine(body);
    }

    return await base.SendAsync(request, cancellationToken);
  }
}
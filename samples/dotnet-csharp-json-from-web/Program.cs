// See https://aka.ms/new-console-template for more information
using GraphConnectorDanToftBlog;
using GraphConnectorDanToftBlog.Extensions;
using GraphConnectorDanToftBlog.Models;
using GraphConnectorDanToftBlog.Util;
using Microsoft.Graph.Models.ExternalConnectors;

Console.WriteLine("Hello, World!");

IEnumerable<Blogpost> posts = await Bloghandler.GetBlogposts();


Console.WriteLine("Checcing if connector already exists");
ExternalConnectionCollectionResponse existingConnectors = await GraphService.Client.External.Connections.GetAsync();
ExternalConnection connection = existingConnectors.Value.FirstOrDefault(x => x.Id == ConnectionConfiguration.ConnectionID);
if (connection == null) {
    Console.WriteLine("No blog connection was found, creating it");
    connection = await GraphService.Client.External.Connections.PostAsync(ConnectionConfiguration.ExternalConnection);

    Console.WriteLine("Provisioning Schema, this will take a few minutes");
    await GraphService.Client.External.Connections[connection.Id].Schema.PatchAsync(ConnectionConfiguration.Schema);
} else {
    Console.WriteLine("Connection already exists, updating it");
    await GraphService.Client.External.Connections[connection.Id].PatchAsync(ConnectionConfiguration.ExternalConnection);
    Console.WriteLine("Updated");
}


Console.WriteLine($"Updating {posts.Count()} posts!");
foreach (Blogpost post in posts) {
    Console.WriteLine($"Updating post {post.title}");
    await GraphService.Client.External.Connections[ConnectionConfiguration.ConnectionID].Items[post.Id].PutAsync(post.ToExternalItem());
 
    //await GraphService.Client.External.Connections[ConnectionConfiguration.ConnectionID].Items[post.Id].DeleteAsync();
    Console.WriteLine("\t > Done");
}

Console.WriteLine("Finished job!");
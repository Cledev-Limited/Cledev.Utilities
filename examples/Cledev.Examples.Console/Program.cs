using Cledev.Core;
using Cledev.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Cledev.Examples.Shared.Streams;

var serviceCollection = new ServiceCollection();
serviceCollection.AddCledevCore(typeof(StreamRequest));
var serviceProvider = serviceCollection.BuildServiceProvider();
var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

await foreach (var response in dispatcher.CreateStream(new StreamRequest()))
{
    Console.Write(response.Text);
}

Console.Read();

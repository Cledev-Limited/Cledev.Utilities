using Cledev.Client.Extensions;
using Cledev.Core.Extensions;
using Cledev.Examples.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddHttpClient("Cledev.Example.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
//    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

//// Supply HttpClient instances that include access tokens when making requests to the server project
//builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Cledev.Example.ServerAPI"));

builder.Services.AddCledevCore();
builder.Services.AddCledevClient(builder.HostEnvironment.BaseAddress, "Cledev.Example.ServerAPI");

builder.Services.AddApiAuthorization();

await builder.Build().RunAsync();

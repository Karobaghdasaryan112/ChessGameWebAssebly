using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using WebAssemblyChessGame.UI;
using WebAssemblyChessGame.UI.ApiServices;
using WebAssemblyChessGame.UI.Contracts;
using WebAssemblyChessGame.UI.ServiceEndpoints;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { });
builder.Services.AddHttpClient("UIClient",client => client.BaseAddress = new Uri(BasePaths.baseUrl));
builder.Services.AddAuthentication();
builder.Services.AddAuthorizationCore();

try
{
    //builder.Services.AddSignalRCore();
    builder.Services.AddScoped(
    sp => new HubConnectionBuilder().
            WithUrl(
                $"{BasePaths.baseUrl}",
                option =>
                {
                    option.Transports = HttpTransportType.WebSockets;

                }).Build());
}
catch(Exception ex)
{
    var exMessage = ex.Message;
}
builder.Services.AddScoped<IdentityService>();
builder.Services.AddScoped<IQueryBuilder, QueryBuilder>();

await builder.Build().RunAsync();

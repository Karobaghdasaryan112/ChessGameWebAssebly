using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebAssemblyChessGame.UI;
using WebAssemblyChessGame.UI.ApiServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { });

builder.Services.AddScoped<IdentityService>();

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebAssemblyChessGame.UI;
using WebAssemblyChessGame.UI.UIServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddUICustomServices();
builder.Services.AddUIDefaultServices();

await builder.Build().RunAsync();

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebAssemblyChessGame.UI;
using WebAssemblyChessGame.UI.UIServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddUICustomServices();

builder.Services.AddUIDefaultServices();

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.MetadataUrl = "https://localhost:7101/.well-known/openid-configuration";
    options.AuthenticationPaths.LogInPath = "https://localhost:7101/authentication/login";
    options.AuthenticationPaths.LogOutPath = "https://localhost:7101/authentication/logout";
    options.AuthenticationPaths.LogInCallbackPath = "https://localhost:7101/authentication/login-callback";
    options.AuthenticationPaths.LogOutCallbackPath = "https://localhost:7101/authentication/logout-callback";
    options.AuthenticationPaths.LogInFailedPath = "https://localhost:7101/authentication/login-failed";
    options.AuthenticationPaths.ProfilePath = "https://localhost:7101/authentication/profile";
    options.ProviderOptions.Authority = "https://localhost:7101";
    options.ProviderOptions.ClientId = "ChessGame-BlazorUI";
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("gateway.read");
    options.ProviderOptions.DefaultScopes.Add("chessgame.read");
});

await builder.Build().RunAsync();
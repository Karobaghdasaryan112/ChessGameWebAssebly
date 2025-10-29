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
    options.AuthenticationPaths.LogInPath = "/authentication/Login";
    options.AuthenticationPaths.LogOutPath = "/authentication/logout";
    options.AuthenticationPaths.LogInCallbackPath = "/authentication/login-callback";
    options.AuthenticationPaths.LogOutCallbackPath = "/authentication/logout-callback";
    options.AuthenticationPaths.LogInFailedPath = "/authentication/login-failed";
    options.AuthenticationPaths.ProfilePath = "/authentication/profile";
    options.ProviderOptions.Authority = "https://localhost:7101";
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.ResponseMode = "query";
    options.ProviderOptions.ClientId = "ChessGame-BlazorUI";
    options.ProviderOptions.DefaultScopes.Add("gateway.read");
    options.ProviderOptions.DefaultScopes.Add("gateway.write");
    options.ProviderOptions.DefaultScopes.Add("chessgame.read");
    options.ProviderOptions.DefaultScopes.Add("chessgame.write");
    options.ProviderOptions.DefaultScopes.Remove("profile");
    options.ProviderOptions.DefaultScopes.Remove("openid");
    

});

await builder.Build().RunAsync();
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
    builder.Configuration.Bind("Oidc", options.ProviderOptions);
    options.ProviderOptions.DefaultScopes.Clear();
    options.ProviderOptions.Authority = "https://localhost:7101";
    options.ProviderOptions.ClientId = "BlazorUI";
    options.ProviderOptions.ResponseType = "code";


    options.ProviderOptions.DefaultScopes.Add("openid");

    // API scopes
    //options.ProviderOptions.DefaultScopes.Add("gateway.read");
    //options.ProviderOptions.DefaultScopes.Add("gateway.write");
    //options.ProviderOptions.DefaultScopes.Add("chessgame.read");
    //options.ProviderOptions.DefaultScopes.Add("chessgame.write");

    // Callback paths
    options.AuthenticationPaths.LogInPath = "/authentication/login";
    options.AuthenticationPaths.LogInCallbackPath = "/authentication/login-callback";
    options.AuthenticationPaths.LogOutCallbackPath = "/authentication/logout-callback";
});

builder.Services.AddHttpClient("Blazor_client", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Blazor_client")
);


await builder.Build().RunAsync();
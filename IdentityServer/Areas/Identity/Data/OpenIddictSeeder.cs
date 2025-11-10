using IdentityServer.Data;
using OpenIddict.Abstractions;

namespace IdentityServer.Areas.Identity.Data
{
    public static class OpenIddictSeeder
    {
        public static async Task Initialize(this IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var applications = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
            var scopes = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();


            //if (await applications.FindByClientIdAsync("BlazorUI") is null)
            //{
            //    await applications.CreateAsync(new OpenIddictApplicationDescriptor()
            //    {
            //        ClientId = "BlazorUI",
            //        DisplayName = "Blazor UI",
            //        ClientType = OpenIddictConstants.ClientTypes.Public,
            //        ApplicationType = OpenIddictConstants.ApplicationTypes.Native,

            //        RedirectUris =
            //        {
            //            new Uri("https://localhost:7225/authentication/login-callback")
            //        },
            //                        PostLogoutRedirectUris =
            //        {
            //            new Uri("https://localhost:7225/authentication/logout-callback")
            //        },
            //                        Permissions =
            //        {
            //            OpenIddictConstants.Permissions.Endpoints.Authorization,
            //            OpenIddictConstants.Permissions.Endpoints.Token, 
            //            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
            //            OpenIddictConstants.Permissions.ResponseTypes.Code,
            //            OpenIddictConstants.Permissions.Prefixes.Endpoint + "openid",
            //            OpenIddictConstants.Permissions.Scopes.Profile
            //        },
            //                        Requirements =
            //        {
            //            OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange 
            //        }
            //    });
            //}
            //if (await scopes.FindByNameAsync("gateway.read") is null)
            //{
            //    await scopes.CreateAsync(new OpenIddictScopeDescriptor()
            //    {
            //        Name = "gateway.read",
            //        DisplayName = "Gateway",
            //        Resources =
            //        {
            //            "gateway"
            //        },

            //    });
            //}

            //if (await scopes.FindByNameAsync("gateway.write") is null)
            //{
            //    await scopes.CreateAsync(new OpenIddictScopeDescriptor()
            //    {
            //        Name = "gateway.write",
            //        DisplayName = "Gateway",
            //        Resources =
            //        {
            //            "gateway"
            //        },

            //    });
            //}

            //if (await scopes.FindByNameAsync("chessgame.write") is null)
            //{
            //    await scopes.CreateAsync(new OpenIddictScopeDescriptor()
            //    {
            //        Name = "chessgame.write",
            //        DisplayName = "ChessGame",
            //        Resources =
            //        {
            //            "chessgame"
            //        },

            //    });
            //}
            //if (await scopes.FindByNameAsync("chessgame.read") is null)
            //{
            //    await scopes.CreateAsync(new OpenIddictScopeDescriptor()
            //    {
            //        Name = "chessgame.read",
            //        DisplayName = "ChessGame",
            //        Resources =
            //        {
            //            "chessgame"
            //        },

            //    });
            //}
        }
    }
}

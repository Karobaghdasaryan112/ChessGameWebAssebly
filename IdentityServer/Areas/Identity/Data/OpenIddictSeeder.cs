using IdentityServer.Data;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

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

            await dbContext.Database.MigrateAsync();

            var clientIds = dbContext.Applications.Select(app => app.ClientId).ToList();
            if (await applications.FindByClientIdAsync("BlazorUI") is null)
            {
                await applications.CreateAsync(new OpenIddictApplicationDescriptor()
                {
                    ClientId = "BlazorUI",
                    ApplicationType = OpenIddictConstants.ApplicationTypes.Native,
                    ClientType = OpenIddictConstants.ClientTypes.Public,
                    DisplayName = "BlazorUI_Display",
                    Permissions =
                    {
                     OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token, 
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.ResponseTypes.Code,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Prefixes.Scope + "openid",
                         //OpenIddictConstants.Permissions.Prefixes.Scope + "gateway.read",
                         //OpenIddictConstants.Permissions.Prefixes.Scope + "gateway.write",
                         //OpenIddictConstants.Permissions.Prefixes.Scope + "chessgame.read",
                         //OpenIddictConstants.Permissions.Prefixes.Scope + "chessgame.write"
                    },
                    RedirectUris =
                    {
                        new Uri("https://localhost:7225/authentication/login-callback")
                    },

                    PostLogoutRedirectUris =
                    {
                        new Uri("https://localhost:7225/authentication/logout-callback")
                    }
                    
                });
            }
            if (await scopes.FindByNameAsync("gateway.read") is null)
            {
                await scopes.CreateAsync(new OpenIddictScopeDescriptor()
                {
                    Name = "gateway.read",
                    DisplayName = "Gateway",
                    Resources =
                    {
                        "gateway"
                    },

                });
            }

            if (await scopes.FindByNameAsync("gateway.write") is null)
            {
                await scopes.CreateAsync(new OpenIddictScopeDescriptor()
                {
                    Name = "gateway.write",
                    DisplayName = "Gateway",
                    Resources =
                    {
                        "gateway"
                    },

                });
            }

            if (await scopes.FindByNameAsync("chessgame.write") is null)
            {
                await scopes.CreateAsync(new OpenIddictScopeDescriptor()
                {
                    Name = "chessgame.write",
                    DisplayName = "ChessGame",
                    Resources =
                    {
                        "chessgame"
                    },

                });
            }
            if (await scopes.FindByNameAsync("chessgame.read") is null)
            {
                await scopes.CreateAsync(new OpenIddictScopeDescriptor()
                {
                    Name = "chessgame.read",
                    DisplayName = "ChessGame",
                    Resources =
                    {
                        "chessgame"
                    },

                });
            }
        }
    }
}

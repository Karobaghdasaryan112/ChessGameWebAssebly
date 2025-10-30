using IdentityServer.Areas.Identity.Data;
using IdentityServer.Areas.Identity.helpers;
using IdentityServer.Data;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text;
using static Microsoft.IO.RecyclableMemoryStreamManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(connectionString);
    options.UseOpenIddict();
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var key = builder.Configuration.GetValue<string>("JwtSettings:SecurityKey");


builder.Services.AddOpenIddict()
    .AddCore(openIdBuilder =>
    {
        openIdBuilder.UseEntityFrameworkCore().UseDbContext<ApplicationDbContext>();
    })
    .AddServer(serverBuilder =>
    {

        serverBuilder
            .AllowRefreshTokenFlow()
            //.AllowClientCredentialsFlow()
            .AllowAuthorizationCodeFlow()
            .RequireProofKeyForCodeExchange()
            .SetTokenEndpointUris("/connect/token")
            .SetAuthorizationEndpointUris("/connect/authorize")
            .SetConfigurationEndpointUris(".well-known/openid-configuration")
            .SetUserInfoEndpointUris("connect/userinfo")
            .RegisterScopes("openid", "profile", "gateway.read", "gateway.write", "chessgame.read", "chessgame.write") //, 

            .UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough();


        serverBuilder
            .AddDevelopmentSigningCertificate()
            .AddDevelopmentEncryptionCertificate();

        serverBuilder.AddSigningKey(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)));

    })
    .AddValidation(validationBuilder =>
    {
        validationBuilder.UseLocalServer();
        validationBuilder.UseAspNetCore();
    });

builder.Services.AddCors(option =>
{
    option.AddPolicy("allow-blazorPolicy", builder =>
    {
        builder.WithOrigins("https://localhost:7225");
        builder.AllowCredentials();
        builder.AllowAnyMethod();
        builder.AllowAnyHeader();
    });
});

//var jwtSettings = configuration.Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.Initialize();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("allow-blazorPolicy");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();

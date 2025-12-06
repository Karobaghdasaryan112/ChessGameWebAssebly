using IdentityServer.Areas.Identity.Data;
using IdentityServer.Areas.Identity.helpers;
using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseOpenIddict();
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var secretKey = builder.Configuration.GetSection("JwtSettings")["SecurityKey"];


builder.Services.AddOpenIddict(builder =>
{
    builder.AddServer(serverBuilder =>
    {
        serverBuilder
        .AllowAuthorizationCodeFlow()
        .RequireProofKeyForCodeExchange()
        .AllowClientCredentialsFlow()
        .SetTokenEndpointUris("/connect/token")
        .SetAuthorizationEndpointUris("connect/authorize")
        .AddDevelopmentEncryptionCertificate()
        .AddDevelopmentSigningCertificate()
        .UseAspNetCore()
        .EnableAuthorizationEndpointPassthrough();

    }).AddCore(coreBuilder =>
    {
        coreBuilder.UseEntityFrameworkCore()
        .UseDbContext<ApplicationDbContext>();

    }).AddValidation(validationBuilder =>
    {
        validationBuilder.UseLocalServer();
        validationBuilder.AddSigningKey(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)));
    });
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
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
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

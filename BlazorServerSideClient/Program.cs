using BlazorServerSideClient.Areas.Identity;
using BlazorServerSideClient.Contracts.Handlers;
using BlazorServerSideClient.Contracts.Requests;
using BlazorServerSideClient.Data;
using BlazorServerSideClient.Data.GameModels;
using BlazorServerSideClient.Data.IdentityModels;
using BlazorServerSideClient.Helpers;
using BlazorServerSideClient.Models;
using BlazorServerSideClient.Services;
using BlazorServerSideClient.Services.Handlers;
using BlazorServerSideClient.Services.Requests;
using ChessGameBlazorClient.ApiServices;
using ChessGameBlazorClient.Contracts;
using ChessGameBlazorClient.ServiceEndpoints;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services
    .AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();

builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();

builder.Services.AddScoped<IQueryBuilder, QueryBuilder>();

builder.Services.AddScoped<ChessGameService>();

builder.Services.AddScoped<BasePaths>();

builder.Services.AddScoped<SignalRService>();
builder.Services.AddScoped<JSRunetimeService>();

builder.Services.AddScoped<GameCircuitState>();

builder.Services.AddScoped<GameCircuitHandler>();
builder.Services.AddScoped<CircuitHandler>(sp =>
    sp.GetRequiredService<GameCircuitHandler>());

builder.Services.AddScoped<IConnectionHandlerService, ConnectionHandlerService>();
builder.Services.AddScoped<IGameHandlerService, GameHandlerService>();
builder.Services.AddScoped<IInvitationHandlerService, InvitationHandlerService>();
builder.Services.AddSingleton<IEmailSender, NullEmasilSender>();
builder.Services.AddScoped<IConnectionReqeustService, ConnectionRequestService>();
builder.Services.AddScoped<IGameRequestService, GameRequestService>();
builder.Services.AddScoped<IInivitationReqeustService, InvitationRequestService>();
builder.Services.AddScoped<IHistoryWidgetRequestService, HistoryWidgetRequestService>();

var chessOrigin = builder.Configuration["ServiceUrls:ChessGameApi"];

if (string.IsNullOrEmpty(chessOrigin))
{
    throw new Exception("CORS Origin 'ServiceUrls:ChessGameApi' is missing from configuration!");
}

builder.Services.AddServerSideBlazor(options =>
{
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(5);
    // How long the server holds the circuit in memory after a disconnect
    options.JSInteropDefaultCallTimeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddCors(options => options.AddPolicy("Default",
    policy =>
        policy
            .WithOrigins(chessOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

builder.Services.AddSignalR()
    .AddNewtonsoftJsonProtocol(options =>
    {
        options.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
    });

builder.Services.AddServerSideBlazor(options =>
{
    options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(5);

    options.JSInteropDefaultCallTimeout = TimeSpan.FromSeconds(30);
}).AddCircuitOptions(options => { options.DetailedErrors = true; });

builder.Services.AddHttpClient("ChessGameBlazorClient.Api", client =>
{
    client.BaseAddress = new Uri($"{new BasePaths(builder.Configuration).BaseUrl}");
    client.Timeout = TimeSpan.FromMinutes(5);
});

builder.Services.AddScoped<ChessGameBlazorClient.ApiServices.IdentityService>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var envName = app.Environment.EnvironmentName;

    // 3. Log the environment info
    logger.LogInformation("========================================");
    logger.LogInformation("Application Starting Up");
    logger.LogInformation("Environment: {EnvironmentName}", envName);
    logger.LogInformation("========================================");
}


//use MigrationApplierService to apply not existing migration in the database
var migrationService = new MigrationApplierService();
migrationService.ApplyMigrations(app.Services);
//


if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        ctx.Context.Response.Headers["Pragma"] = "no-cache";
        ctx.Context.Response.Headers["Expires"] = "0";
    }
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// app.UseHttpsRedirection();
app.UseCors();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
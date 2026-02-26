using BlazorServerSideClient.Areas.Identity;
using BlazorServerSideClient.Contracts.Handlers;
using BlazorServerSideClient.Contracts.Requests;
using BlazorServerSideClient.Data;
using BlazorServerSideClient.Services;
using BlazorServerSideClient.Services.Handlers;
using BlazorServerSideClient.Services.Requests;
using ChessGame.Core.Services.MediatR.Handlers.Commands;
using ChessGameBlazorClient.ApiServices;
using ChessGameBlazorClient.Contracts;
using ChessGameBlazorClient.ServiceEndpoints;
using ChessGameBlazorClient.UI.Services;
using FluentValidation;
using IdentityService.Domain.Domain;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharedResources.DTOs.ChessGameDTOs.RequestDTOs.ConnectionRequestDTOs.GameRequestDTOs;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<ApplicationUser>(options => { options.SignIn.RequireConfirmedAccount = true; })
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();

builder.Services.AddScoped<IQueryBuilder, QueryBuilder>();

builder.Services.AddScoped<ChessGameService>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SignalRService>();
builder.Services.AddScoped<JSRunetimeService>();

builder.Services.AddScoped<IConnectionHandlerService, ConnectionHandlerService>();
builder.Services.AddScoped<IGameHandlerService, GameHandlerService>();
builder.Services.AddScoped<IInvitationHandlerService, InvitationHandlerService>();

builder.Services.AddScoped<IConnectionReqeustService, ConnectionRequestService>();
builder.Services.AddScoped<IGameRequestService, GameRequestService>();
builder.Services.AddScoped<IInivitationReqeustService, InvitationRequestService>();
builder.Services.AddScoped<IHistoryWidgetRequestService, HistoryWidgetRequestService>();

builder.Services.AddSignalR()
    .AddNewtonsoftJsonProtocol(options =>
    {
        options.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
    });

builder.Services.AddHttpClient("ChessGameBlazorClient.Api", client =>
{
    client.BaseAddress = new Uri($"{BasePaths.baseUrl}");
});
builder.Services.AddScoped<ChessGameBlazorClient.ApiServices.IdentityService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

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
app.UseCors(options => {
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

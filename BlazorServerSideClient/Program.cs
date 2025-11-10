using BlazorServerSideClient.Areas.Identity;
using BlazorServerSideClient.Data;
using BlazorServerSideClient.Services;
using ChessGameBlazorClient.Contracts;
using ChessGameBlazorClient.ServiceEndpoints;
using ChessGameBlazorClient.UI.Services;
using IdentityService.Domain.Domain;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<ApplicationUser>(options => { options.SignIn.RequireConfirmedAccount = true; })
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<IQueryBuilder, QueryBuilder>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SignalRService>();
builder.Services.AddSignalR();
builder.Services.AddHttpClient("ChessGameBlazorClient.Api", client =>
{
    client.BaseAddress = new Uri($"{BasePaths.baseUrl}" ?? throw new InvalidOperationException("API base URL not configured."));
});
builder.Services.AddScoped<ChessGameBlazorClient.ApiServices.IdentityService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

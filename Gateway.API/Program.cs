using Gateway.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUI_CORS", policy =>
    {
        policy.WithOrigins("https://localhost:7225")
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var proxySection = builder.Configuration.GetSection("ReverseProxy");

builder.Services.AddReverseProxy()
    .LoadFromConfig(proxySection);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<LoggingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowUI_CORS");
app.UseAuthorization();
app.MapControllers();
app.UseEndpoints(endpoints => { endpoints.MapReverseProxy(); });
app.MapReverseProxy();

app.Use(async (context, next) =>
{
    await next();

    var proxyFeature = context.Features.Get<Yarp.ReverseProxy.Model.IReverseProxyFeature>();
    if (proxyFeature != null)
    {
        Console.WriteLine($"? Routed to cluster: {proxyFeature.Cluster}");
        Console.WriteLine($"? Destination address: {proxyFeature.ProxiedDestination?.DestinationId}");
        Console.WriteLine($"? Final path: {context.Request.Path}");
    }
});

app.Run();

using RuleCollections.API.GRPCServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/* Routing mode */
builder.Services.AddControllers();
/* Grpc mode
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 51000, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});
builder.Services.AddGrpc(); 
*/
builder.Services.AddDaprClient();

var app = builder.Build();
onHttpApp(app);
//onGrpcApp(app);
app.Run();

static WebApplication onHttpApp(WebApplication app)
{
    // Configure the HTTP request pipeline.
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapSubscribeHandler();
        endpoints.MapControllers();
    });

    return app;
}

static WebApplication onGrpcApp(WebApplication app)
{
    app.UseRouting();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGrpcService<RuleService>();    // Communication with gRPC endpoints must be made through a gRPC client.
    });

    return app;
}

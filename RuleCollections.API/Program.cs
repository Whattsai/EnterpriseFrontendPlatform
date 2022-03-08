using Microsoft.AspNetCore.Server.Kestrel.Core;
using RuleCollections.API.GRPCServices;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 51000, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});
builder.Services.AddGrpc();
builder.Services.AddDaprClient();

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseCloudEvents();
app.UseRouting();
//app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    //endpoints.MapSubscribeHandler();
    //endpoints.MapControllers();
    endpoints.MapGrpcService<RuleService>();    // Communication with gRPC endpoints must be made through a gRPC client.
});

app.Run();

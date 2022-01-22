var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDaprClient();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();

app.Run();

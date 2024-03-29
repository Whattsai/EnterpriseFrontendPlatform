using Healthchecks.Dapr;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json.Serialization;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);
var allowSpecificOrigins = "allowSpecificOrigins";
// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 此值定義 JSON 是否應該使用美化顯示。 根據預設，JSON 會序列化，而不會有任何額外的空白字元。
        options.JsonSerializerOptions.WriteIndented = true;
        // 判斷是否要在序列化及還原序列化期間處理欄位
        options.JsonSerializerOptions.IncludeFields = true;
        // 此值決定屬性的名稱在還原序列化期間是否使用不區分大小寫的比較。
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        // 維持大小寫
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        //允許額外符號
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        // 取消Unicode編碼
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

        options.JsonSerializerOptions.ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip;
        options.JsonSerializerOptions.MaxDepth = 10;
        options.JsonSerializerOptions.UnknownTypeHandling = System.Text.Json.Serialization.JsonUnknownTypeHandling.JsonNode;
    })
    //.AddNewtonsoftJson(options =>
    //{
    //    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    //})
    ;
builder.Services.AddDaprClient();
builder.Services.AddHttpClient();
builder.Services
    .AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddCheck<DaprHealthCheck>("dapr");

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:8080")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseAuthorization();
app.MapControllers();
app.UseCors(allowSpecificOrigins);
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/apphc", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});
app.MapHealthChecks("/daprhc", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("dapr")
});

app.Run();

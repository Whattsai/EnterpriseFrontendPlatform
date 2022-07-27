using Newtonsoft.Json.Serialization;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(
    options =>
    {
        // ���ȩw�q JSON �O�_���Өϥά�����ܡC �ھڹw�]�AJSON �|�ǦC�ơA�Ӥ��|�������B�~���ťզr���C
        options.JsonSerializerOptions.WriteIndented = true;
        // �P�_�O�_�n�b�ǦC�Ƥ��٭�ǦC�ƴ����B�z���
        options.JsonSerializerOptions.IncludeFields = true;
        // ���ȨM�w�ݩʪ��W�٦b�٭�ǦC�ƴ����O�_�ϥΤ��Ϥ��j�p�g������C
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        // �����j�p�g
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        //���\�B�~�Ÿ�
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        // ����Unicode�s�X
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);

        options.JsonSerializerOptions.ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip;
        options.JsonSerializerOptions.MaxDepth = 10;
        options.JsonSerializerOptions.UnknownTypeHandling = System.Text.Json.Serialization.JsonUnknownTypeHandling.JsonNode;
        //options.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();

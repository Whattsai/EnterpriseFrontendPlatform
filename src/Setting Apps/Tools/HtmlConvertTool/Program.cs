using HtmlAgilityPack;
using HtmlConvertTool;
using HtmlConvertTool.DataClass;
using Microsoft.Extensions.Configuration;

//設定檔初始化
var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile($"appsettings.json").Build();
AppSettings.OutputFilePath = configuration.GetSection("AppSettings:OutputFilePath").Value;
AppSettings.LoadHtmlFilePath = configuration.GetSection("AppSettings:LoadHtmlFilePath").Value;
AppSettings.AxiosPost = configuration.GetSection("AppSettings:AxiosPost").Value;

//執行轉檔
DyWebConvert convert = new DyWebConvert();
convert.ExecuteConvert();

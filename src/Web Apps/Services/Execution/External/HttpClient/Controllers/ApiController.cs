using Common.Model;
using Dapr.Client;
using HttpClientAPI.Model;
using Microsoft.AspNetCore.Mvc;
using SJ.Convert;

namespace HttpClientAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly DaprClient _daprClient;

        public ApiController(ILogger<ApiController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        [HttpPost("APIGet")]
        public async Task<object> APIGet(EFPRequest request)
        {
            StreamReader r = new StreamReader($"SettingData/API/{request.ID}.json");
            string jsonString = r.ReadToEnd();
            APISettingModel apiSetting = JsonTrans.ToModelOrDefault<APISettingModel>(jsonString)!;

            var url = StringParse.GetCombinedString(apiSetting.URL, request.Data);
            var result = await _httpClient.GetAsync(url);
            var respose = await result.Content.ReadAsStringAsync();
            return respose;
        }
    }
}
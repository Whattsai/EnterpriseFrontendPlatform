using Calculate.API.Model;
using Common.Model;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SJ.Convert;

namespace Calculate.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataProcessController : ControllerBase
    {
        private readonly DaprClient _daprClient;

        private readonly ILogger<DataProcessController> _logger;

        public DataProcessController(ILogger<DataProcessController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GroupBy")]
        public object Get(EFPRequest request)
        {
            StreamReader r2 = new StreamReader($"SettingData/DataProcess/{request.ID}.json");
            var jsonString2 = r2.ReadToEnd();
            var gropByModel = JsonConvert.DeserializeObject<GroupByModel>(jsonString2)!;

            

            if (!string.IsNullOrEmpty(gropByModel.TargetList))
            {

            }

            object a = request.Data;
            var hs = gropByModel.TargetList.Split('.');
            foreach (var h in hs)
            {
                
            }

            Dictionary<string, List<object>> ans = new Dictionary<string, List<object>>();
            List<object> list = request.Data;
            foreach (var item in list)
            {
                var dic = DictionaryEx.ToDictionary<string>(item);
                if (ans.ContainsKey(dic[gropByModel.GroupKey]))
                {
                    ans[gropByModel.GroupKey].Add(item);
                }
            }

            return "QQ";
        }
    }
}
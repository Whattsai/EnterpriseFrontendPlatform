using Common.Model;
using Common.Module;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ManagementCollections.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AggregateSettingController : EFPControllerBase
    {
        private readonly DaprClient _daprClient;

        public AggregateSettingController(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [HttpGet("Build")]
        public async Task<SortedDictionary<string, List<string>>> Build(string aggregateID)
        {
            // Aggregate´ú¸Õ¸ê®Æ
            StreamReader r = new StreamReader($"SettingData/AggregateSetting/{aggregateID}.json");
            string jsonString = r.ReadToEnd();
            SortedDictionary<string, List<string>> mapNextAction = JsonConvert.DeserializeObject<SortedDictionary<string, List<string>>>(jsonString)!;

            await _daprClient.SaveStateAsync("statestore", aggregateID, mapNextAction, new StateOptions() { Consistency = ConsistencyMode.Strong });

            return mapNextAction;
        }
    }
}
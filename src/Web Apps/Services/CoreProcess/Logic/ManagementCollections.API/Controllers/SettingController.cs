using Common.Model;
using Common.Module;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ManagementCollections.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SettingController : EFPControllerBase
    {
        private readonly DaprClient _daprClient;

        public SettingController(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }


        [HttpGet("Build")]
        public async Task<ActionResult> BuildAggregate(string aggregateID)
        {
            // Aggregate´ú¸Õ¸ê®Æ
            StreamReader r = new StreamReader($"{aggregateID}.json");
            string jsonString = r.ReadToEnd();
            SortedDictionary<string, List<string>> mapNextAction = JsonConvert.DeserializeObject<SortedDictionary<string, List<string>>>(jsonString)!;

            await _daprClient.SaveStateAsync("statestore", aggregateID, mapNextAction, new StateOptions() { Consistency = ConsistencyMode.Strong });

            apiResponse = new EFPapiResponse(EnumEFPResultCode.Success, $"Build {aggregateID} into state", mapNextAction);

            _logger.LogInformation(JsonConvert.SerializeObject(apiResponse));

            return Ok(apiResponse);
        }
    }
}
using Aggregate.Module;
using Common.Model;
using Dapr.Client;
using GrpcWheather;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Web.Actions.Aggregator.Controllers
{
    /// <summary>
    /// CalculateController
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AggregateController : ControllerBase
    {
        private readonly DaprClient _daprClient;

        public AggregateController(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [HttpGet("Build")]
        public async Task<ActionResult> Build(string aggregateID)
        {
            // Aggregate測試資料
            StreamReader r = new StreamReader($"SettingData/{aggregateID}.json");
            string jsonString = r.ReadToEnd();
            SortedDictionary<string, List<string>> mapNextAction = JsonConvert.DeserializeObject<SortedDictionary<string, List<string>>>(jsonString)!;

            await _daprClient.SaveStateAsync("statestore", aggregateID, mapNextAction, new StateOptions() { Consistency = ConsistencyMode.Strong });
            return Ok($"Build {aggregateID} into statestore");
        }

        [HttpGet("GetTree")]
        public async Task<ActionResult> Get(string id)
        {
            var result = await _daprClient.InvokeMethodAsync<HelloReply>(HttpMethod.Get, "logicapi", $"action/getTree?id={id}");
            return Ok(result);
        }

        [HttpPost("GO")]
        public async Task<ActionResult> Go(EFPRequest request)
        {
            request.Token = new Guid();
            var mapNextAction = await _daprClient.GetStateAsync<SortedDictionary<string, List<string>>>("statestore", request.ID);
            //GetBonusRequest getBonusRequest = new GetBonusRequest(0, "19285", 2021, "TW");

            // 初始化AggregateModule
            AggregateModule aggregateModule = new AggregateModule(mapNextAction, _daprClient, request);
            Task.Run (()=>aggregateModule.Go()).Wait();

            //var result = await _daprClient.InvokeMethodAsync<string>(HttpMethod.Get, "logicapi", "action/RunTree");
            return Ok(aggregateModule);
        }

        [HttpGet("DaprPostState")]
        public async Task<ActionResult> PostDaprStateAsync()
        {
            var result = await _daprClient.InvokeMethodAsync<HelloReply>(HttpMethod.Post, "logicapi", "rule/poststate");
            return Ok(result);
        }

        [HttpGet("DaprDeleteState")]
        public async Task<ActionResult> DeleteStateAsync()
        {
            var result = await _daprClient.InvokeMethodAsync<HelloReply>(HttpMethod.Delete, "logicapi", "rule/deletestate");
            return Ok(result);
        }

        [HttpGet("DaprTestPub")]
        public async Task<ActionResult> TestPubAsync()
        {
            var result = await _daprClient.InvokeMethodAsync<WeatherForecast>(HttpMethod.Get, "logicapi", "rule/testpubself");
            return Ok(result);
        }
    }
}

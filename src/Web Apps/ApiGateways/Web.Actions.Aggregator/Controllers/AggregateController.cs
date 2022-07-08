using ActionEngine.DataClass.Model;
using Aggregate.Module;
using Dapr.Client;
using GrpcWheather;
using HR.Model.Bounts;
using Microsoft.AspNetCore.Mvc;

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
            var mapNextAction = new SortedDictionary<string, List<string>>();
            mapNextAction.Add("A", new List<string> { "B", "D" });
            mapNextAction.Add("B", new List<string> { });
            mapNextAction.Add("C", new List<string> { "D" });
            mapNextAction.Add("D", new List<string> { });

            await _daprClient.SaveStateAsync("statestore", aggregateID, mapNextAction, new StateOptions() { Consistency = ConsistencyMode.Strong });


            //var result = await _daprClient.InvokeMethodAsync<Dictionary<string, ActionModel>>(HttpMethod.Get, "logicapi", "action/buidtree");
            return Ok("OK");
        }

        [HttpGet("GetTree")]
        public async Task<ActionResult> Get(string id)
        {
            var result = await _daprClient.InvokeMethodAsync<HelloReply>(HttpMethod.Get, "logicapi", $"action/getTree?id={id}");
            return Ok(result);
        }

        [HttpGet("Run")]
        public async Task<ActionResult> Run(string aggregateID)
        {
            var mapNextAction = await _daprClient.GetStateAsync<SortedDictionary<string, List<string>>>("statestore", aggregateID);
            GetBonusRequest getBonusRequest = new GetBonusRequest(0, "19285", 2021, "TW");

            // 初始化AggregateModule
            AggregateModule aggregateModule = new AggregateModule(mapNextAction, _daprClient);
            aggregateModule.Run();

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

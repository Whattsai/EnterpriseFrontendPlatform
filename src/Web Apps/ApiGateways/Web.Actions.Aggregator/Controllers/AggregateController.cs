using ActionEngine.DataClass.Model;
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

        [HttpGet("Buidtree")]
        public async Task<ActionResult> Buidtree()
        {
            var result = await _daprClient.InvokeMethodAsync<Dictionary<string, ActionModel>>(HttpMethod.Get, "logicapi", "action/buidtree");
            return Ok(result);
        }

        [HttpGet("GetTree")]
        public async Task<ActionResult> GetTree(string id)
        {
            var result = await _daprClient.InvokeMethodAsync<HelloReply>(HttpMethod.Get, "logicapi", $"action/getTree?id={id}");
            return Ok(result);
        }

        [HttpGet("RunTree")]
        public async Task<ActionResult> RunTree()
        {
            GetBonusRequest getBonusRequest = new GetBonusRequest(0, "19285", 2021, "TW");

            var result = await _daprClient.InvokeMethodAsync<string>(HttpMethod.Get, "logicapi", "action/RunTree");
            return Ok(result);
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

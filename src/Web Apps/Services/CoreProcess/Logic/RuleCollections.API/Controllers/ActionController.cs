using ActionEngine.DataClass.Model;
using ActionEngine.Module;
using Dapr;
using Dapr.Client;
using GrpcWheather;
using HR.Model.Bounts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace RuleCollections.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ActionController : ControllerBase
    {
        private readonly ILogger<ActionModel> _logger;
        private readonly DaprClient _daprClient;

        public ActionController(ILogger<ActionModel> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        #region Service invocation
        [HttpGet("BuidTree")]
        public async Task<Dictionary<string, ActionModel>> BuidTree()
        {
            try
            {
                var result = await _daprClient.InvokeMethodAsync<Dictionary<string, ActionModel>>(HttpMethod.Get, "logicapi", "test");
                await _daprClient.SaveStateAsync<Dictionary<string, ActionModel>>("statestore", "C1", result, new StateOptions() { Consistency = ConsistencyMode.Strong });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString());
                return null;
            }
        }

        [HttpGet("GetTree")]
        public async Task<HelloReply> GetTree(string id)
        {
            var result = await _daprClient.GetStateAsync<string>("statestore", id);
            return new HelloReply { Message = result };
        }

        [HttpGet("RunTree")]
        public async Task<HelloReply> RunTree()
        {
            GetBonusRequest getBonusRequest = new GetBonusRequest(0, "19285", 2021, "TW");
            var tree = await _daprClient.GetStateAsync<Dictionary<string, ActionModel>>("statestore", "C1");
            //var result = await _daprClient.InvokeMethodAsync<Dictionary<string, ActionModel>, string> (HttpMethod.Post, "logicapi", "action/go", tree);

            AggregateModule aggregateModule = new AggregateModule();
            aggregateModule.Go(tree, getBonusRequest);

            return new HelloReply { Message = JsonConvert.SerializeObject(aggregateModule.OutModel) };
        }

        //[HttpGet("Go")]
        //public async Task<HelloReply> Go(Dictionary<string, ActionModel> actions, object request)
        //{
        //    ConditionModule conditionModule = new ConditionModule();
        //    conditionModule.Go(actions);
        //}

        #endregion

        #region State
        [HttpGet("GetState")]
        public async Task<HelloReply> GetStateAsync()
        {
            var result = await _daprClient.GetStateAsync<string>("statestore", "guid");
            return new HelloReply { Message = result };
        }

        [HttpPost("PostState")]
        public async Task<HelloReply> PostStateAsync()
        {
            await _daprClient.SaveStateAsync<string>("statestore", "guid", Guid.NewGuid().ToString(), new StateOptions() { Consistency = ConsistencyMode.Strong });
            return new HelloReply { Message = "done" };
        }

        [HttpDelete("DeleteState")]
        public async Task<HelloReply> DeleteStateAsync()
        {
            await _daprClient.DeleteStateAsync("statestore", "guid");
            return new HelloReply { Message = "done" };
        }

        [HttpPost("PostStateWithTag")]
        public async Task<ActionResult> PostStateWithTagAsync()
        {
            var (value, etag) = await _daprClient.GetStateAndETagAsync<string>("statestore", "guid");
            await _daprClient.TrySaveStateAsync<string>("statestore", "guid", Guid.NewGuid().ToString(), etag);
            return Ok("done");
        }

        [HttpDelete("DeleteStateWithTag")]
        public async Task<ActionResult> DeleteStateWithTagAsync()
        {
            var (value, etag) = await _daprClient.GetStateAndETagAsync<string>("statestore", "guid");
            return Ok(await _daprClient.TryDeleteStateAsync("statestore", "guid", etag));
        }
        #endregion

        #region PubSub

        [HttpGet("TestPubSelf")]
        public async Task<WeatherForecast> TestPubSelfAsync()
        {
            var data = new WeatherForecast
            {
                Summary = "都沒你的甜兒",
                TemperatureC = 50,
                Date = DateTime.Now
            };

            _logger.LogInformation("start publish");
            await _daprClient.PublishEventAsync("pubsub", "rule", data);
            _logger.LogInformation("end publish");
            return data;
        }

        [Topic("pubsub", "rule")]
        [HttpPost("TestSubSelf")]
        public async Task<ActionResult> TestSubSelfAsync(WeatherForecast data, [FromServices] DaprClient daprClient)
        {
            _logger.LogInformation("success into sub!");
            _logger.LogInformation($"start sub：{System.Text.Json.JsonSerializer.Serialize(data)}");
            _logger.LogInformation("end!");
            return Ok("done");
        }
        #endregion
    }
}

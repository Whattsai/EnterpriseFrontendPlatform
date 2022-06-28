using ActionEngine.DataClass.Model;
using ActionEngine.Module;
using Dapr;
using Dapr.Client;
using GrpcWheather;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionModel> BuidTree()
        {         
            //try
            //{
            //    ConditionModule _conditionModule = new ConditionModule();
            //    Dictionary<string, ActionModel> root = new Dictionary<string, ActionModel>();
            //    root.Add("C1", new ActionModel(
            //        new Condition("mapperKey", _conditionModule.BuildTree(new object[] { "&", "!", "EmpID", null, "!", "Year", null })),
            //        new ExecuteAction("GetAPI呼叫GetAnnualSalary", EnumActionType.ApiGet, "GetAnnualSalary"),
            //        new Condition("AnnualSalaryRsponse", _conditionModule.BuildTree(new object[] { "!", "ReturnData", null }))
            //    ));

            //    return root;
            //    //return new DaprResponse() { ResultCode = DaprResultCode.OK, DetailJsonString = JsonToken.StartObject(root) };
            //    //var result = await _daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(HttpMethod.Get, "logicapi", "weatherforecast");
            //    //return result;
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogWarning(ex.ToString());
            //    return null;
            //}

            try
            {
                var result = await _daprClient.InvokeMethodAsync<ActionModel>(HttpMethod.Get, "logicapi", "test");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString());
                return null;
            }
        }
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

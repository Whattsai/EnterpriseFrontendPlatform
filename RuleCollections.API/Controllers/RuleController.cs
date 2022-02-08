using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace RuleCollections.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RuleController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly DaprClient _daprClient;

        public RuleController(ILogger<WeatherForecastController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        #region Service invocation
        [HttpGet("TestSelfCall")]
        public async Task<IEnumerable<WeatherForecast>> TestSelfCallAsync()
        {
            try
            {
                var result = await _daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(HttpMethod.Get, "logicapi", "weatherforecast");
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
        public async Task<ActionResult> GetStateAsync()
        {
            var result = await _daprClient.GetStateAsync<string>("statestore", "guid");
            return Ok(result);
        }

        [HttpPost("PostState")]
        public async Task<ActionResult> PostStateAsync()
        {
            await _daprClient.SaveStateAsync<string>("statestore", "guid", Guid.NewGuid().ToString(), new StateOptions() { Consistency = ConsistencyMode.Strong });
            return Ok("done");
        }

        [HttpDelete("DeleteState")]
        public async Task<ActionResult> DeleteStateAsync()
        {
            await _daprClient.DeleteStateAsync("statestore", "guid");
            return Ok("done");
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
            _logger.LogInformation($"start sub：{JsonSerializer.Serialize(data)}");

            //Stream stream = Request.Body;
            //byte[] buffer = new byte[Request.ContentLength.Value];
            //stream.Position = 0L;
            //stream.ReadAsync(buffer, 0, buffer.Length);
            //string content = Encoding.UTF8.GetString(buffer);
            //_logger.LogInformation("topicStatus" + content);
            _logger.LogInformation("end!");
            return Ok("done");
        }
        #endregion
    }
}

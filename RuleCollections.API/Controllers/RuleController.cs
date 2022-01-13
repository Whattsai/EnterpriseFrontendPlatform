using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> TestSelfCallAsync()
        {
            var result = await _daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(HttpMethod.Get, "logicapi", "weatherforecast");
            return result;
        }
        #endregion

        #region State
        [HttpGet]
        public async Task<ActionResult> GetStateAsync()
        {
            var result = await _daprClient.GetStateAsync<string>("statestore", "guid");
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> PostStateAsync()
        {
            await _daprClient.SaveStateAsync<string>("statestore", "guid", Guid.NewGuid().ToString(), new StateOptions() { Consistency = ConsistencyMode.Strong });
            return Ok("done");
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteStateAsync()
        {
            await _daprClient.DeleteStateAsync("statestore", "guid");
            return Ok("done");
        }

        [HttpPost]
        public async Task<ActionResult> PostStateWithTagAsync()
        {
            var (value, etag) = await _daprClient.GetStateAndETagAsync<string>("statestore", "guid");
            await _daprClient.TrySaveStateAsync<string>("statestore", "guid", Guid.NewGuid().ToString(), etag);
            return Ok("done");
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteStateWithTagAsync()
        {
            var (value, etag) = await _daprClient.GetStateAndETagAsync<string>("statestore", "guid");
            return Ok(await _daprClient.TryDeleteStateAsync("statestore", "guid", etag));
        }
        #endregion

        #region PubSub

        public async Task<WeatherForecast> TestPubSelfAsync()
        {
            var data = new WeatherForecast
            {
                Summary = "都沒你的甜兒",
                TemperatureC = 105,
                Date = DateTime.Now
            };

            await _daprClient.PublishEventAsync("calculate", "rule", data);
            return data;
        }

        [Topic("calculate", "rule")]
        [HttpPost]
        public async Task<ActionResult> TestSubSelfAsync(WeatherForecast weatherForecast, [FromServices] DaprClient daprClient)
        {
            Stream stream = Request.Body;
            byte[] buffer = new byte[Request.ContentLength.Value];
            stream.Position = 0L;
            stream.ReadAsync(buffer, 0, buffer.Length);
            string content = Encoding.UTF8.GetString(buffer);
            _logger.LogInformation("topicStatus" + content);
            return Ok(content);
        }
        #endregion
    }
}

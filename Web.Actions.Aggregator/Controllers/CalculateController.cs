using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace Web.Actions.Aggregator.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CalculateController : ControllerBase
    {
        private readonly DaprClient _daprClient;

        public CalculateController(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [HttpGet("DaprClientWithDI")]
        public async Task<ActionResult> GetDaprClientWithDIResultAsync()
        {
            var result = await _daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(HttpMethod.Get, "logicapi", "testselfcall");
            return Ok(result);
        }
    }
}

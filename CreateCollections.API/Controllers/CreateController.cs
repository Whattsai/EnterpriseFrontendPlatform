using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace CreateCollections.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CreateController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly DaprClient _daprClient;

        public CreateController(ILogger<WeatherForecastController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        [Topic("calculate", "rule")]
        [HttpPost]
        public async Task<ActionResult> TestSubCreateAsync(WeatherForecast weatherForecast, [FromServices] DaprClient daprClient)
        {
            Stream stream = Request.Body;
            byte[] buffer = new byte[Request.ContentLength.Value];
            stream.Position = 0L;
            stream.ReadAsync(buffer, 0, buffer.Length);
            string content = Encoding.UTF8.GetString(buffer);
            _logger.LogInformation("topicStatus" + content);
            return Ok(content);
        }
    }
}

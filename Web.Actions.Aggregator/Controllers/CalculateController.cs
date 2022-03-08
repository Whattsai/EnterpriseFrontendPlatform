using Dapr.Client;
using GrpcWheather;
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

        [HttpGet("DaprServiceInvoke")]
        public async Task<ActionResult> GetDaprClientWithDIResultAsync()
        {
            var result = await _daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(HttpMethod.Get, "logicapi", "rule/testselfcall");
            return Ok(result);
        }

        [HttpGet("DaprTestPub")]
        public async Task<ActionResult> TestPubAsync()
        {
            var result = await _daprClient.InvokeMethodAsync<HelloReply>(HttpMethod.Get, "logicapi", "rule/testpubself");
            return Ok(result);
        }

        [HttpGet("DaprServiceInvokeGrpc")]
        public async Task<ActionResult> DaprClientGRPCWithDIResultAsync()
        {
            var result = await _daprClient.InvokeMethodGrpcAsync<HelloRequest, HelloReply>("logicapi", "testselfcall", new HelloRequest { Name = "aaa" });
            return Ok(result);
        }

        [HttpGet("DaprGetStateGrpc")]
        public async Task<ActionResult> GetDaprGRPCStateAsync()
        {
            var result = await _daprClient.InvokeMethodGrpcAsync<HelloReply>("logicapi", "rule/getstate");
            return Ok(result);
        }

        [HttpGet("DaprPostStateGrpc")]
        public async Task<ActionResult> PostDaprGRPCStateAsync()
        {
            var result = await _daprClient.InvokeMethodGrpcAsync<HelloReply>("logicapi", "rule/poststate");
            return Ok(result);
        }

        [HttpGet("DaprDeleteStateGrpc")]
        public async Task<ActionResult> DeleteGRPCStateAsync()
        {
            var result = await _daprClient.InvokeMethodGrpcAsync<HelloReply>("logicapi", "rule/deletestate");
            return Ok(result);
        }

        [HttpGet("DaprTestPubGrpc")]
        public async Task<ActionResult> TestPubGRPCAsync()
        {
            var result = await _daprClient.InvokeMethodGrpcAsync<HelloReply>("logicapi", "rule/testpubself");
            return Ok(result);
        }
    }
}

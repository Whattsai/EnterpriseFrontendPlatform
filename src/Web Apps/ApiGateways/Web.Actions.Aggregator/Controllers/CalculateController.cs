using Dapr.Client;
using GrpcWheather;
using Microsoft.AspNetCore.Mvc;

namespace Web.Actions.Aggregator.Controllers
{
    /// <summary>
    /// CalculateController
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class CalculateController : ControllerBase
    {
        private readonly DaprClient _daprClient;
        private readonly HttpClient _httpClient;

        public CalculateController(DaprClient daprClient, HttpClient httpClient)
        {
            _daprClient = daprClient;
            _httpClient = httpClient;
        }

        [HttpGet("DaprServiceInvoke")]
        public async Task<ActionResult> GetDaprClientWithDIResultAsync()
        {
            var result = await _daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(HttpMethod.Get, "logicapi", "rule/testselfcall");
            return Ok(result);
        }

        [HttpGet("DaprGetState")]
        public async Task<ActionResult> GetDaprStateAsync()
        {
            var result = await _daprClient.InvokeMethodAsync<HelloReply>(HttpMethod.Get, "logicapi", "rule/getstate");
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

        [HttpGet("HttpClient")]
        public async Task<ActionResult> GetHttpClientResultAsync()
        {
            var result = await _httpClient.GetAsync("http://host.docker.internal:6001/api/PersonalData/GetBonus?year=2020&employeeID=15235&companyCode=098");
            var resultContent = string.Format("result is {0} {1}", result.StatusCode, await result.Content.ReadAsStringAsync());
            return Ok(resultContent);
        }

        [HttpGet("DaprBindingHttp")]
        public async Task<ActionResult> BindingHttpAsync()
        {
            var metadata = new Dictionary<string, string>()
            {
                ["path"] = "/Rule/TestSelfCall"
            };
            var data = new Dictionary<string, string>(){};
            var result = await _daprClient.InvokeBindingAsync<Dictionary<string,string>, object>("ehr_httpbinding", "get", data, metadata);
            return Ok(result);
        }

        /* 使用Grpc通訊
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
        */
    }
}

using Aggregate.Module;
using Common.Model;
using Dapr.Client;
using GrpcWheather;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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

        [HttpPost("GO")]
        public async Task<ActionResult> Go(EFPRequest request)
        {
            // 生成該次請求request token
            request.Token = new Guid();

            // 取得參數
            var mapNextAction = await _daprClient.GetStateAsync<SortedDictionary<string, List<string>>>("statestore", request.ID);

            if (mapNextAction == null)
            {
                mapNextAction = await _daprClient.InvokeMethodAsync<SortedDictionary<string, List<string>>>(HttpMethod.Get, "managementapi", $"aggregatesetting/build?aggregateID={ request.ID }");
            }

            // 初始化AggregateModule
            AggregateModule aggregateModule = new AggregateModule(mapNextAction, _daprClient, request);
            Task.Run(() => aggregateModule.Go()).Wait();

            return Ok(aggregateModule);
        }
    }
}

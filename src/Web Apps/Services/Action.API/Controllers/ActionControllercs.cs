using ActionEngine.DataClass.Model;
using Aggregate.Model;
using Common.Model;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace Action.API.Controllers
{
    /// <summary>
    /// CalculateController
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ActionController : ControllerBase
    {
        private readonly DaprClient _daprClient;

        public ActionController(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        [HttpPost("Go")]
        public async Task<StateModel> Go(EFPRequest request)
        {
            var cache = await _daprClient.GetStateAsync<List<ActionModel>>("statestore", request.ID);
            if (cache == null)
            {
                cache = await _daprClient.InvokeMethodAsync<List<ActionModel>>(HttpMethod.Get, "managementapi", "build");
            }

            return new StateModel(true, request.Data);
        }
    }
}

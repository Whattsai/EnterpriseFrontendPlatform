using ActionEngine.DataClass.Model;
using ActionEngine.Module;
using Aggregate.Model;
using Common.Model;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using SJ.Convert;

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
            var actions = await _daprClient.GetStateAsync<List<ActionModel>>("statestore", request.ID);
            if (actions == null)
            {
                actions = await _daprClient.InvokeMethodAsync<List<ActionModel>>(HttpMethod.Get, "managementapi", $"actionsetting/build?actionID={request.ID}");
            }

            ActionModule actionModule = new ActionModule(_daprClient);

            foreach (var action in actions)
            {
                request.Data = actionModule.Go(action, request.Data);
            }

            return new StateModel(true, request.Data);
        }

        [HttpPost("Test")]
        public async Task<StateModel> Test(string id)
        {

            return new StateModel(true, id);
        }
    }
}

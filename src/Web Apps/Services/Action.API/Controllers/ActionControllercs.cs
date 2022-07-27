using ActionEngine.DataClass.Model;
using ActionEngine.Module;
using Aggregate.Model;
using Common.Model;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using SJ.Convert;
using System.Text;
using System.Text.Json;

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
        public ActionResult Go(EFPRequest request)
        {
            var actions = Task.Run(()=> _daprClient.GetStateAsync<List<ActionModel>>("statestore", request.ID)).Result;
            if (actions == null)
            {
                actions = Task.Run(()=> _daprClient.InvokeMethodAsync<List<ActionModel>>(HttpMethod.Get, "managementapi", $"actionsetting/build?actionID={request.ID}")).Result ;
            }

            ActionModule actionModule = new ActionModule(_daprClient);

            Dictionary<string, object> tmp = DictionaryEx.ToDictionary<object>(request.Data);

            foreach (var action in actions)
            {
                tmp = actionModule.Go(action, tmp);
            }

            return  Content(JsonSerializer.Serialize(new StateModel(true, tmp["Data"])), "application/json", Encoding.UTF8);
            //return Ok(new StateModel(true, tmp["Data"]));
        }

        [HttpPost("Test")]
        public async Task<StateModel> Test(string id)
        {

            return new StateModel(true, id);
        }
    }
}

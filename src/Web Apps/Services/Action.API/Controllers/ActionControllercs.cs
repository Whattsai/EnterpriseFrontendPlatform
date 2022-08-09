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

            Dictionary<string, object> requestModel = DictionaryEx.ToDictionary<object>(request.Data);
            Dictionary<string, object> ans = new Dictionary<string, object>();

            foreach (var action in actions)
            {
                ans = actionModule.Go(action, requestModel);
                //ans.Add(action.Name, actionModule.Go(action, requestModel));
            }

            return  Content(JsonSerializer.Serialize(new StateModel(true, ans)), "application/json", Encoding.UTF8);
        }

        [HttpPost("Build")]
        public bool Build(Dictionary<string, dynamic> stateDatas)
        {
            foreach (var data in stateDatas)
            {
                Task.Run(()=> _daprClient.SaveStateAsync("statestore", data.Key, data.Value, new StateOptions() { Consistency = ConsistencyMode.Strong })) ;
            }

            Task.WaitAll();

            return true;
        }
    }
}

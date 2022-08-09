using ActionEngine.DataClass.Model;
using ActionEngine.Module;
using Common.Model;
using Common.Module;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ManagementCollections.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActionSettingController : EFPControllerBase
    {
        private readonly DaprClient _daprClient;
        private readonly HttpClient _httpClient;

        public ActionSettingController(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }


        [HttpGet("Build")]
        public async Task<List<ActionModel>> Build(string actionID)
        {
            // Action´ú¸Õ¸ê®Æ
            StreamReader r = new StreamReader($"SettingData/Action/{actionID}.json");
            string jsonString = r.ReadToEnd();
            List<ActionSettingModel> actionSetting = JsonConvert.DeserializeObject<List<ActionSettingModel>>(jsonString)!;

            ConditionModule _conditionModule = new ConditionModule();

            List<ActionModel> actions = new List<ActionModel>();
            foreach (var item in actionSetting)
            {
                StreamReader r2 = new StreamReader($"SettingData/Execution/{item.ExecuteActionID}.json");
                var jsonString2 = r2.ReadToEnd();
                var execution = JsonConvert.DeserializeObject<ExecuteAction>(jsonString2)!;

                actions.Add(new ActionModel(
                    item.Name,
                    new Condition(item.BeforeExecuteCondition.MapperKey, _conditionModule.BuildTree(item.BeforeExecuteCondition.ConditionSetting)),
                    new ExecuteAction(execution.Name, execution.ActionType, execution.Key),
                    new Condition(item.AfterExecuteCondition.MapperKey, _conditionModule.BuildTree(item.AfterExecuteCondition.ConditionSetting))
                    ));
            }

            //await _daprClient.SaveStateAsync("statestore", actionID, actions, new StateOptions() { Consistency = ConsistencyMode.Strong });

            Dictionary<string, List<ActionModel>> request = new Dictionary<string, List<ActionModel>>();
            request.Add(actionID, actions);
            await _daprClient.InvokeMethodAsync<Dictionary<string, List<ActionModel>>, bool>(HttpMethod.Post, $"action", $"action/build", request);

            return actions;
        }

        [HttpGet]
        public Dictionary<string, ActionModel> Get()
        {
            ConditionModule _conditionModule = new ConditionModule();
            Dictionary<string, ActionModel> root = new Dictionary<string, ActionModel>();
            root.Add("C1", new ActionModel(
                "C1_Action",
                new Condition("mapperKey", _conditionModule.BuildTree(new object[] { "&", "!", "EmpID", null, "!", "Year", null })),
                new ExecuteAction("GetAPI©I¥sGetAnnualSalary", EnumActionType.ApiGet, "GetAnnualSalary"),
                new Condition("AnnualSalaryRsponse", _conditionModule.BuildTree(new object[] { "!", "ReturnData", null }))
            ));

            return root;
        }
    }
}
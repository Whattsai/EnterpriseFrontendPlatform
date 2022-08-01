using ActionEngine.DataClass.Interface;
using ActionEngine.DataClass.Model;
using Dapr.Client;
using SJ.Convert;
using SJ.ObjectMapper.Module;

namespace ActionEngine.Module
{
    public class ActionModule : IActionModule<object, Dictionary<string, object>>
    {
        public readonly DaprClient _daprClient;

        public ActionModule(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        ConditionModule conditionModule = new ConditionModule();

        public void BeforeExecuteCondition(Condition condition, Dictionary<string, object> request)
        {
            // TODO 尚須處理 request mapping
            //condition.OutModel = request;
            if (!string.IsNullOrEmpty(condition.MapperKey))
            {
                StreamReader r = new StreamReader($"SettingData/Mapper/{condition.MapperKey}.json");
                string jsonString = r.ReadToEnd();
                condition.RequestModel = new Mapper().GetTreeMapResult(jsonString, request, new Dictionary<string, object>());
            }
            else{
                condition.RequestModel = request;
            }

            // 判斷請求參數是否正確
            condition.IsOK = conditionModule.Go(condition.ConditionTree, condition.RequestModel, condition.RequestModel);
        }

        public object ExecuteAction(ExecuteAction action, Dictionary<string, object> inModel)
        {
            return new ExcutionModule(_daprClient).Go(action, inModel);
        }

        public void AfterExecuteCondition(Condition condition, object response)
        {
            Dictionary<string, object> dict = DictionaryEx.ToDictionary<object>(response);

            // 判斷請求參數是否正確
            condition.IsOK = conditionModule.Go(condition.ConditionTree, dict, dict);

            // TODO 尚須處理 request mapping
            if (!string.IsNullOrEmpty(condition.MapperKey))
            {
                StreamReader r = new StreamReader($"SettingData/Mapper/{condition.MapperKey}.json");
                string jsonString = r.ReadToEnd();
                condition.OutModel = new Mapper().GetTreeMapResult(jsonString, response, new Dictionary<string, object>());
            }
            else
            {
                condition.OutModel = dict;
            }
        }

        public Dictionary<string, object> Go(ActionModel action, Dictionary<string, object> request)
        {
            // 如beforeCondition未通過，跳過
            BeforeExecuteCondition(action.BeforeExecuteCondition, request);

            // bool? 執行成功: true, 失敗: false, 未執行: null
            if (action.BeforeExecuteCondition.IsOK == true)
            {
                // 執行action
                AfterExecuteCondition(action.AfterExecuteCondition, ExecuteAction(action.ExecuteAction, request));
                return action.AfterExecuteCondition.OutModel;
            }

            // BeforeExecuteConditiong失敗時，需要request用FailMapperKey作mapper處理
            return request;
        }
    }
}

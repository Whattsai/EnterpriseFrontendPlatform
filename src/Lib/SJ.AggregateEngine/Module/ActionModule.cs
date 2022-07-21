using ActionEngine.DataClass.Interface;
using ActionEngine.DataClass.Model;
using Dapr.Client;
using SJ.Convert;
using SJ.ObjectMapper.Module;

namespace ActionEngine.Module
{
    public class ActionModule : IActionModule<Dictionary<string, object>, Dictionary<string, object>>
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
            condition.OutModel = request;

            // 判斷請求參數是否正確
            condition.IsOK = conditionModule.Go(condition.ConditionTree, condition.OutModel, condition.OutModel);
        }

        public Dictionary<string, object> ExecuteAction(ExecuteAction action, Dictionary<string, object> inModel)
        {
            return new ExcutionModule(_daprClient).Go(action, inModel);
        }

        public void AfterExecuteCondition(Condition condition, Dictionary<string, object> request)
        {
            // 判斷請求參數是否正確
            condition.IsOK = conditionModule.Go(condition.ConditionTree, request, request);

            // TODO 尚須處理 request mapping
            StreamReader r = new StreamReader($"SettingData/Mapper/{condition.MapperKey}.json");
            string jsonString = r.ReadToEnd();
            condition.OutModel = new Mapper().GetTreeMapResult<Dictionary<string, object>>(jsonString, request, new Dictionary<string, object>());

            //condition.OutModel = request;
        }

        public object Go(ActionModel action, object request)
        {
            var dictionaryData = DictionaryEx.ToDictionary<object>(request)!;
            // 如beforeCondition未通過，跳過
            BeforeExecuteCondition(action.BeforeExecuteCondition, dictionaryData);

            // bool? 執行成功: true, 失敗: false, 未執行: null
            if (action.BeforeExecuteCondition.IsOK == true)
            {
                // 執行action
                AfterExecuteCondition(action.AfterExecuteCondition, ExecuteAction(action.ExecuteAction, dictionaryData));
                return action.AfterExecuteCondition.OutModel;
            }

            // BeforeExecuteConditiong失敗時，需要request用FailMapperKey作mapper處理
            return request;
        }
    }
}

using ActionEngine.DataClass.Model;
using Newtonsoft.Json;
using SJ.Convert;

namespace ActionEngine.Module
{
    public class AggregateModule
    {
        /// <summary>
        /// 出參數
        /// </summary>
        public object? OutModel { get; set; }

        ActionModule actionModule = new ActionModule();
        ConditionModule conditionModule = new ConditionModule();
        public void Go(Dictionary<string, ActionModel> aggregeat, object request, bool isStart = true)
        {
            if (isStart)
            {
                OutModel = request;
            }

            foreach (var action in aggregeat)
            {
                // 尋找先行action
                if (action.Value.PreAction.Count > 0)
                {
                    Go(action.Value.PreAction, request, false);
                }

                Dictionary<string, object> tmp = JsonTrans.ToModelOrDefault<Dictionary<string, object>>(JsonConvert.SerializeObject(action.Value.PreAction));

                /** 找到最優先且未執行的action */
                if (action.Value.PreActionCondition?.ConditionTree == null || conditionModule.Go(action.Value.PreActionCondition.ConditionTree, tmp, tmp))
                {
                    OutModel = actionModule.Go(action.Value, DictionaryEx.ToDictionary<object>(OutModel));       
                }
            }
        }
    }
}

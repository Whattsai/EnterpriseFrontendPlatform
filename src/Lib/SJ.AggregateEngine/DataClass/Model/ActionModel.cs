
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ActionEngine.DataClass.Model
{
    /// <summary>
    /// 執行動作的設定結構
    /// </summary>
    public class ActionModel
    {
        [JsonConstructor]
        public ActionModel() { }

        public ActionModel(
            Condition beforeExecuteCondition,
            ExecuteAction executeAction,
            Condition afterExecuteCondition,
            Condition? preActionCondition = null,
            Dictionary<string, ActionModel>? preAction = null,
            string? failMapperKey = null
            )
        {
            this.BeforeExecuteCondition = beforeExecuteCondition;
            this.ExecuteAction = executeAction;
            this.AfterExecuteCondition = afterExecuteCondition;
            this.PreActionCondition = preActionCondition;
            this.PreAction = preAction?? new Dictionary<string, ActionModel>();
            this.FailMapperKey = failMapperKey;
        }

        /// <summary>
        /// 是否執行判斷式
        /// </summary>
        public Condition BeforeExecuteCondition { get; set; }

        /// <summary>
        /// 執行動作
        /// </summary>
        public ExecuteAction ExecuteAction { get; set; }

        /// <summary>
        /// 失敗時的處理
        /// </summary>
        public string? FailMapperKey { get; set; }

        /// <summary>
        /// 執行後回傳的判斷結果
        /// </summary>
        public Condition AfterExecuteCondition { get; set; }

        /// <summary>
        /// 判斷之前的action，決定是否執行此action
        /// </summary>
        public Condition? PreActionCondition { get; set; }

        /// <summary>
        /// 前置Action
        /// </summary>
        public Dictionary<string, ActionModel> PreAction { get; set; }

        private delegate object ExctionAction(Excution map);
    }
}

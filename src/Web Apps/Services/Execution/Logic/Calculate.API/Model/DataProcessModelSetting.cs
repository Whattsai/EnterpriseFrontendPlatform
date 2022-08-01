namespace Calculate.API.Model
{
    public class GroupByModel
    {
        /// <summary>
        /// 要被Group的清單 請從Request.Data中一路索引到[]所在參數，如Request.Data本身就為[]，則可為空
        /// </summary>
        public string TargetList { get; set; }

        /// <summary>
        /// 依此參數去做group
        /// </summary>
        public string GroupKey { get; set; }
    }
}

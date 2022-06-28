using ActionEngine.DataClass.Model;

namespace ActionEngine.Module
{
    internal class API
    {
        public API()
        {
        }

        internal Dictionary<string, object> ApiGet(ExecuteAction action, Dictionary<string, object> inModel)
        {
            StreamReader r = new StreamReader($"SettingData/ExternalAPI/{action.Key}.json");
            string jsonString = r.ReadToEnd();
            return SJ.Convert.JsonTrans.ToModelOrDefault<Dictionary<string, object>>(jsonString)?? new Dictionary<string, object>();
        }
    }
}
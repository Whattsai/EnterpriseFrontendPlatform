using Newtonsoft.Json;

namespace SJ.Convert
{
    /// <summary>
    /// Json序列化Extension
    /// </summary>
    public static class JsonTrans
    {
        /// <summary>
        /// 轉換為指定Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T? ToModelOrDefault<T>(string jsonString)
        {
            return jsonString == null ? Activator.CreateInstance<T>() : JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}

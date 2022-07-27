using ActionEngine.DataClass.Model;
using Common.Model;
using Dapr.Client;
using SJ.Convert;

namespace ActionEngine.Module
{
    internal class API
    {
        private readonly DaprClient _daprClient;

        public API(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public Dictionary<string, object> ApiGet(ExecuteAction action, object inModel)
        {
            EFPRequest request = new EFPRequest()
            {
                ID = action.Key,
                Data = inModel
            };
            var result = Task.Run(() => _daprClient.InvokeMethodAsync<EFPRequest, Dictionary<string, object>>(HttpMethod.Post, "httpclient", "api/apiget" ,request)).Result;

            return result;
        }
    }
}
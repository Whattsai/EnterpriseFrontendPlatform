using Dapr.Client;
using DaprExtention.DataClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaprExtention.Module
{
    public class DaprAPI
    {

        private readonly DaprClient _daprClient;

        public DaprAPI(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }
        

        public async Task<T?> InvokeMethodAsync<T>(HttpMethod httpMethod, string appId, string methodName)
        {
            DaprResponse result = await _daprClient.InvokeMethodAsync<DaprResponse>(HttpMethod.Get, "logicapi", "action/buidtree");

            if(result.ResultCode != DaprResultCode.OK)
            {
                throw new Exception($"【{result.ResultCode}】{result.Message}");
            }

            return JsonConvert.DeserializeObject<T>(result.DetailJsonString);
        }
    }
}

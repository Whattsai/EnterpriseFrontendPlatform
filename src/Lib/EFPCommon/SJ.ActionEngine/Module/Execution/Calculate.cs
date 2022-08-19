using ActionEngine.DataClass.Model;
using Common.Model;
using Dapr.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJ.ActionEngine.Module.Execution
{
    public class Calculate
    {
        private readonly DaprClient _daprClient;

        public Calculate(DaprClient daprClient)
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
            var result = Task.Run(() => _daprClient.InvokeMethodAsync<EFPRequest, Dictionary<string, object>>(HttpMethod.Post, "httpclient", "GroupBy", request)).Result;

            return result;
        }
    }
}

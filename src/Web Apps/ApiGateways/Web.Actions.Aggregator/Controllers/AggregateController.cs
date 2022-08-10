using Aggregate.Model;
using Aggregate.Module;
using Common.Model;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using SJ.ObjectMapper.Module;
using System.Collections.Concurrent;

namespace Web.Actions.Aggregator.Controllers
{
    /// <summary>
    /// CalculateController
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AggregateController : ControllerBase
    {
        private readonly DaprClient _daprClient;

        private Dictionary<string, MapperWay> _IMapper { get; set; }

        private delegate object MapperWay(ConcurrentDictionary<string, StateModel> stateModel, EFPRequest request);

        public AggregateController(DaprClient daprClient)
        {
            _daprClient = daprClient;

            _IMapper = new Dictionary<string, MapperWay>()
            {
                {"EFP_GetBonusAndSalary", new MapperWay(baResponseMapper) }
            };
        }

        [HttpPost("GO")]
        public async Task<ActionResult> Go(EFPRequest request)
        {
            // 生成該次請求request token
            request.Token = new Guid();

            // 取得參數
            var info = await _daprClient.GetStateAsync<AggregateInfo>("statestore", request.ID);

            if (info == null)
            {
                info = await _daprClient.InvokeMethodAsync<AggregateInfo>(HttpMethod.Get, $"{request.Service}maintainservice", $"aggregatesetting/build?aggregateID={ request.ID }");
            }

            // 初始化AggregateModule
            AggregateModule aggregateModule = new AggregateModule(info.ActionsControl, _daprClient, request);
            Task.Run(() => aggregateModule.Go()).Wait();

            // 整理response，如客製字典內無對應，則使用預設mapper
            if (string.IsNullOrEmpty(info.ResponseMapperJson))
            {
                return Ok(Task.Run(() => baResponseMapper(aggregateModule.MapStateModel, request)).Result);
            }

            //StreamReader r = new StreamReader($"settingdata/mapper/{request.ID}.json");
            //string jsonstring = r.ReadToEnd();

            Dictionary<string, object> inmodel = new Dictionary<string, object>();
            foreach (var item in aggregateModule.MapStateModel)
            {
                inmodel.Add(item.Key, item.Value);
            }

            return Ok(new Mapper().GetTreeMapResult(info.ResponseMapperJson, inmodel, new Dictionary<string, object>()));
        }

        [HttpPost("Build")]
        public bool Build(AggregateInfo info)
        {
            Task.Run(() => _daprClient.SaveStateAsync("statestore", $"{info.ID}", info, new StateOptions() { Consistency = ConsistencyMode.Strong })).Wait();
            return true;
        }

        /// <summary>
        /// 使用BA撰寫的客製轉置邏輯(透過dapr呼叫BA Service)
        /// </summary>
        /// <param name="stateModel"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<object> baResponseMapper(ConcurrentDictionary<string, StateModel> stateModel, EFPRequest request)
        {
            return await _daprClient.InvokeMethodAsync<ConcurrentDictionary<string, StateModel>, object>(HttpMethod.Post, $"{request.Service}moduleservice", $"{request.ID}/responsemapper", stateModel);
        }

        public class AggregateInfo
        {
            public string ID { get; set; }

            public SortedDictionary<string, List<string>> ActionsControl { get; set; }

            public string? ResponseMapperJson { get; set; }
        }
    }
}

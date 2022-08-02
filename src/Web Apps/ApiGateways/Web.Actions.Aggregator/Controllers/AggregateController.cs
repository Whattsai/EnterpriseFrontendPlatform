using Aggregate.Model;
using Aggregate.Module;
using Common.Model;
using Dapr.Client;
using HR.Moudule;
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

        private delegate object MapperWay(ConcurrentDictionary<string, StateModel> stateModel);

        public AggregateController(DaprClient daprClient)
        {
            _daprClient = daprClient;

            _IMapper = new Dictionary<string, MapperWay>()
            {
                {"Aggregate_EFP.GetBounsAndSalary", new MapperWay(new GetBounsAndSalaryMapper().Go) }
            };
        }

        [HttpPost("GO")]
        public async Task<ActionResult> Go(EFPRequest request)
        {
            // 生成該次請求request token
            request.Token = new Guid();

            // 取得參數
            var aggregateSetting = await _daprClient.GetStateAsync<SortedDictionary<string, List<string>>>("statestore", request.ID);

            if (aggregateSetting == null)
            {
                aggregateSetting = await _daprClient.InvokeMethodAsync<SortedDictionary<string, List<string>>>(HttpMethod.Get, "managementapi", $"aggregatesetting/build?aggregateID={ request.ID }");
            }

            // 初始化AggregateModule
            AggregateModule aggregateModule = new AggregateModule(aggregateSetting, _daprClient, request);
            Task.Run(() => aggregateModule.Go()).Wait();

            // 整理response，如客製字典內無對應，則使用預設mapper
            if (_IMapper.ContainsKey(request.ID))
            {
                return Ok(new MapperWay(_IMapper[request.ID]).Invoke(aggregateModule.MapStateModel));
            }

            StreamReader r = new StreamReader($"settingdata/mapper/{request.ID}.json");
            string jsonstring = r.ReadToEnd();

            Dictionary<string, object> inmodel = new Dictionary<string, object>();
            foreach (var item in aggregateModule.MapStateModel)
            {
                inmodel.Add(item.Key, item.Value);
            }

            return Ok(new Mapper().GetTreeMapResult(jsonstring, inmodel, new Dictionary<string, object>()));
        }
    }
}

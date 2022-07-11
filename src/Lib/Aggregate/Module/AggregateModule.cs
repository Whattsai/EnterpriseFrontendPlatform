using Aggregate.Model;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;
using Dapr.Client;
using System.Collections.Concurrent;

namespace Aggregate.Module
{
    public class AggregateModule
    {
        private readonly DaprClient _daprClient;

        private readonly Guid? _requestID;

        /// <summary>
        /// 每個ActionNode可連至的下個ActionNodeList
        /// </summary>
        public SortedDictionary<string, List<string>> mapNextAction { get; set; }

        /// <summary>
        /// 每個ActionNode的前置條件
        /// </summary>
        public SortedDictionary<string, List<string>> mapPreAction { get; set; }

        /// <summary>
        /// 回傳資料成否與否與ResponseData存放空間(供Aggregate進行DP運算)
        /// </summary>
        public ConcurrentDictionary<string, StateModel> mapStateModel { get; set; }

        /// <summary>
        /// 執行緒管理map
        /// </summary>
        public Dictionary<string, Task> mapTask { get; set; }

        /// <summary>
        /// Task鎖
        /// </summary>
        public Object thisLock { get; set; } = new Object();

        /// <summary>
        /// 服務請求資料
        /// </summary>
        public object RequestData { get; set; }

        /// <summary>
        /// Initail TESTDATA
        /// </summary>
        public AggregateModule(SortedDictionary<string, List<string>> mapNextAction, DaprClient daprClient, Guid? requestID)
        {
            _requestID = requestID;
            _daprClient = daprClient;
            mapTask = new Dictionary<string, Task>();
            mapStateModel = new ConcurrentDictionary<string, StateModel>();
            this.mapNextAction = mapNextAction;

            _computPreAction();
        }

        /// <summary>
        /// Aggregate開始
        /// </summary>
        public async void Run()
        {
            foreach (var preAction in mapPreAction)
            {
                Action<object?> action = (object? obj) => RunAction(preAction.Key);
                Task task = new Task(action, preAction.Key);
                mapTask.Add(preAction.Key, task);
            }

            List<Task> firstRun = new List<Task>();
            foreach (var preAction in mapPreAction)
            {
                if (preAction.Value.Count == 0)
                {
                    firstRun.Add(mapTask[preAction.Key]);
                    mapTask[preAction.Key].Start();
                }
            }

            Task.WaitAll(firstRun.ToArray());
            await Task.WhenAll(mapTask.Values.ToArray());
        }

        /// <summary>
        /// Action執行，並回傳結果
        /// </summary>
        public void RunAction(string actionKey)
        {
            //var result = await _daprClient.InvokeMethodAsync<StateModel>(HttpMethod.Post, "logicapi", "action/buidtree");
            Dictionary<string, bool> actionIsSuccessResponse = new Dictionary<string, bool>()
            {
                { "A", true },
                { "B", true },
                { "C", false },
                { "D", true },
            };

            var apiResponse = new StateModel(actionIsSuccessResponse[actionKey], $"response{actionKey}");

            lock (thisLock)
            {
                mapStateModel.TryAdd(actionKey, apiResponse);

                foreach (var nextNodeKey in mapNextAction[actionKey])
                {
                    bool isReadyToGo = true;
                    foreach (var preNode in mapPreAction[nextNodeKey])
                    {
                        if (!mapStateModel.ContainsKey(preNode) || !mapStateModel[preNode].IsSuccess)
                        {
                            isReadyToGo = false;
                            break;
                        }
                    }

                    if (isReadyToGo)
                    {
                        mapTask[nextNodeKey].Start();
                    }
                }
            }
        }

        private void _computPreAction()
        {
            // 重新宣告PreAction並實作物件
            mapPreAction = new SortedDictionary<string, List<string>>();
            foreach (var nextAction in mapNextAction)
            {
                mapPreAction.Add(nextAction.Key, new List<string> { });
            }

            // 透過nextAction計算PreAction
            foreach (var nextAction in mapNextAction)
            {
                foreach (var nodeKey in nextAction.Value)
                {
                    mapPreAction[nodeKey].Add(nextAction.Key);
                }
            }
        }
    }
}

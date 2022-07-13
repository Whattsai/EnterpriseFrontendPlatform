using Aggregate.Model;
using Dapr.Client;
using System.Collections.Concurrent;

namespace Aggregate.Module
{
    public class AggregateModule
    {
        private readonly DaprClient _daprClient;

        /// <summary>
        /// 每個ActionNode可連至的下個ActionNodeList
        /// </summary>
        public SortedDictionary<string, List<string>> MapNextAction { get; set; }

        /// <summary>
        /// 每個ActionNode的前置條件
        /// </summary>
        public SortedDictionary<string, List<string>> MapPreAction { get; set; }

        /// <summary>
        /// 回傳資料成否與否與ResponseData存放空間(供Aggregate進行DP運算)
        /// </summary>
        public ConcurrentDictionary<string, StateModel> MapStateModel { get; set; }

        /// <summary>
        /// 執行緒管理map
        /// </summary>
        public Dictionary<string, Task> MapTask { get; set; }

        /// <summary>
        /// Task鎖
        /// </summary>
        private Object thisLock { get; set; } = new Object();

        /// <summary>
        /// 服務請求資料
        /// </summary>
        public EFPRequest Request { get; set; }

        /// <summary>
        /// Initail TESTDATA
        /// </summary>
        public AggregateModule(SortedDictionary<string, List<string>> mapNextAction, DaprClient daprClient, EFPRequest request)
        {
            _daprClient = daprClient;
            MapTask = new Dictionary<string, Task>();
            MapStateModel = new ConcurrentDictionary<string, StateModel>();
            this.MapNextAction = mapNextAction;
            Request = request;

            _computPreAction();
        }

        /// <summary>
        /// Aggregate開始
        /// </summary>
        public void Go()
        {
            foreach (var preAction in MapPreAction)
            {
                Action action = () => GoAction(preAction.Key);
                Task task = new Task(action);
                MapTask.Add(preAction.Key, task);
            }

            List<Task> firstRun = new List<Task>();
            foreach (var preAction in MapPreAction)
            {
                if (preAction.Value.Count == 0)
                {
                    firstRun.Add(MapTask[preAction.Key]);
                    MapTask[preAction.Key].Start();
                }
            }

            //Task.WaitAll(firstRun.ToArray());
            Task.WhenAll(MapTask.Values.ToArray()).Wait();
        }

        /// <summary>
        /// Action執行，並回傳結果
        /// </summary>
        public void GoAction(string actionKey)
        {
            var actionRequest = new EFPRequest(Request, actionKey);
            var apiResponse = Task.Run(()=> _daprClient.InvokeMethodAsync<object, StateModel>(HttpMethod.Post, "logicapi", "action/run", actionRequest)).Result;

            lock (thisLock)
            {
                MapStateModel.TryAdd(actionKey, apiResponse);

                foreach (var nextNodeKey in MapNextAction[actionKey])
                {
                    bool isReadyToGo = true;
                    foreach (var preNode in MapPreAction[nextNodeKey])
                    {
                        if (!MapStateModel.ContainsKey(preNode) || !MapStateModel[preNode].IsSuccess)
                        {
                            isReadyToGo = false;
                            break;
                        }
                    }

                    if (isReadyToGo)
                    {
                        MapTask[nextNodeKey].Start();
                    }
                }
            }
        }

        private void _computPreAction()
        {
            // 重新宣告PreAction並實作物件
            MapPreAction = new SortedDictionary<string, List<string>>();
            foreach (var nextAction in MapNextAction)
            {
                MapPreAction.Add(nextAction.Key, new List<string> { });
            }

            // 透過nextAction計算PreAction
            foreach (var nextAction in MapNextAction)
            {
                foreach (var nodeKey in nextAction.Value)
                {
                    MapPreAction[nodeKey].Add(nextAction.Key);
                }
            }
        }
    }
}

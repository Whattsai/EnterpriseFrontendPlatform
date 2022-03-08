using Dapr.AppCallback.Autogen.Grpc.v1;
using Dapr.Client;
using Dapr.Client.Autogen.Grpc.v1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcWheather;
using System.Text.Json;

namespace RuleCollections.API.GRPCServices
{
    public class RuleService : AppCallback.AppCallbackBase
    {
        private readonly ILogger<HelloReply> _logger;
        private readonly DaprClient _daprClient;

        public RuleService(ILogger<HelloReply> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        public override async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
        {
            var response = new InvokeResponse();
            switch (request.Method)
            {
                case "testselfcall":
                    var input = request.Data.Unpack<HelloRequest>();
                    var result = await _daprClient.InvokeMethodGrpcAsync<HelloRequest, HelloReply>("logicapi", "weatherforecast", new HelloRequest { Name = "bbb" });
                    response.Data = Any.Pack(result);
                    break;
                case "weatherforecast":
                    response.Data = Any.Pack(new HelloReply { Message = "okokokokokokokokok" });
                    break;
                case "rule/getstate":
                    var state = await _daprClient.GetStateAsync<string>("statestore", "guid");
                    response.Data = Any.Pack(new HelloReply { Message = state });
                    break;
                case "rule/poststate":
                    await _daprClient.SaveStateAsync<string>("statestore", "guid", Guid.NewGuid().ToString(), new Dapr.Client.StateOptions() { Consistency = ConsistencyMode.Strong });
                    response.Data = Any.Pack(new HelloReply { Message = "poststate ok" });
                    break;
                case "rule/deletestate":
                    await _daprClient.DeleteStateAsync("statestore", "guid");
                    response.Data = Any.Pack(new HelloReply { Message = "deletestate ok" });
                    break;
                case "rule/testpubself":
                    var data = new WeatherForecast
                    {
                        Summary = "都沒你的甜兒",
                        TemperatureC = 50,
                        Date = DateTime.Now
                    };
                    _logger.LogInformation("start publish");
                    await _daprClient.PublishEventAsync("pubsub", "rule", data);
                    _logger.LogInformation("end publish");
                    response.Data = Any.Pack(new HelloReply { Message = "pub go!" });
                    break;
            }
            return response;
        }

        public override Task<ListTopicSubscriptionsResponse> ListTopicSubscriptions(Empty request, ServerCallContext context)
        {
            var result = new ListTopicSubscriptionsResponse();
            result.Subscriptions.Add(new TopicSubscription
            {
                PubsubName = "pubsub",
                Topic = "rule"
            });
            return Task.FromResult(result);
        }

        public override async Task<TopicEventResponse> OnTopicEvent(TopicEventRequest request, ServerCallContext context)
        {
            var response = new TopicEventResponse();
            switch (request.Topic)
            {
                case "rule":
                    _logger.LogInformation("success into sub!");
                    _logger.LogInformation($"start sub：{JsonSerializer.Serialize(request.Data)}");
                    _logger.LogInformation("end!");
                    break;
            }
            
            response.Status = TopicEventResponse.Types.TopicEventResponseStatus.Success;
            return await Task.FromResult(default(TopicEventResponse));
        }
    }
}

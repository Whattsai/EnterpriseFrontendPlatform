using ActionEngine.DataClass.Model;
using ActionEngine.Module;
using Microsoft.AspNetCore.Mvc;

namespace RuleCollections.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionModel Get()
        {
            ConditionModule _conditionModule = new ConditionModule();
            return new ActionModel(
                new Condition("mapperKey", _conditionModule.BuildTree(new object[] { "&", "!", "EmpID", null, "!", "Year", null })),
                new ExecuteAction("GetAPI©I¥sGetAnnualSalary", EnumActionType.ApiGet, "GetAnnualSalary"),
                new Condition("AnnualSalaryRsponse", _conditionModule.BuildTree(new object[] { "!", "ReturnData", null }))
            );

            //ConditionModule _conditionModule = new ConditionModule();
            //Dictionary<string, ActionModel> root = new Dictionary<string, ActionModel>();
            //root.Add("C1", new ActionModel(
            //    new Condition("mapperKey", _conditionModule.BuildTree(new object[] { "&", "!", "EmpID", null, "!", "Year", null })),
            //    new ExecuteAction("GetAPI©I¥sGetAnnualSalary", EnumActionType.ApiGet, "GetAnnualSalary"),
            //    new Condition("AnnualSalaryRsponse", _conditionModule.BuildTree(new object[] { "!", "ReturnData", null }))
            //));

            //return new WeatherForecast()
            //{
            //    Summary = "AAA"
            //};

            //return new AAAA("A1", "»¡©ú");
        }
    }
}
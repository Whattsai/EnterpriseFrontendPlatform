using Microsoft.AspNetCore.Mvc;

namespace HR_PersonalData.Controllers
{
    public class HRReponseBase<T>
    {
        public string ReturnCode { get; set; } = "A0";
        public string? ReturnMessage { get; set; }
        public T? ReturnData { get; set; }
    }

    public class BonusCompay
    {
        public string? CompanyTypeCode { get; set; }
        public string? CompanyTypeName { get; set; }
    }

    public class Bonus
    {
        public string? Year { get; set; }
        public string? BonusType { get; set; }
        public string? SalaryItemCategoryI { get; set; }
        public string? SalaryItemCategoryIName { get; set; }
        public string? CompanyCode { get; set; }
        public string? EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        public string? JobTitle { get; set; }
        public string? BankAccount1 { get; set; }
        public string? BankAccount2 { get; set; }
        public string? DueAmount { get; set; }
        public string? Deduction { get; set; }
        public string? NetTotal { get; set; }
        public string? MemoCode { get; set; }
        public string? Remark { get; set; }
        public string? SalaryItemCategoryIII { get; set; }
        public string? SalaryItemCategoryIIIName { get; set; }
        public string? SalaryItemRemark { get; set; }
        public string? PlusorMinus { get; set; }
        public string? Salary_Amt { get; set; }
        public string? Stock_Amt { get; set; }
        public string? Stock_Prices { get; set; }
        public string? MatchTag { get; set; }
        public string? CreatedDate { get; set; }
        public string? LastChgDate { get; set; }
    }

    public class AnnualSalary
    {

    }

    [ApiController]
    [Route("api/[controller]")]
    public class PersonalDataController : ControllerBase
    {
        private readonly ILogger<PersonalDataController> _logger;

        public PersonalDataController(ILogger<PersonalDataController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GetBonusCompay")]
        public HRReponseBase<List<BonusCompay>> GetBonusCompay(string year, string employeeID)
        {
            return new HRReponseBase<List<BonusCompay>>()
            {
                ReturnCode = "A0",
                ReturnMessage = "有資料/寫入成功/執行成功",
                ReturnData = new List<BonusCompay>()
                {
                    new BonusCompay() {
                        CompanyTypeCode = "098",
                        CompanyTypeName = "玉山金融控股股份有限公司" }
                }
            };
        }

        [HttpGet("GetBonus")]
        public HRReponseBase<List<Bonus>> GetBonus(string year, string employeeID, string companyCode)
        {
            return new HRReponseBase<List<Bonus>>()
            {
                ReturnCode = "A0",
                ReturnMessage = "有資料/寫入成功/執行成功",
                ReturnData = new List<Bonus>() {
                    new Bonus() {
                    }
                }
            };
        }

        [HttpGet("GetAnnualSalary")]
        public HRReponseBase<List<AnnualSalary>> GetAnnualSalary(string year, string employeeID)
        {
            return new HRReponseBase<List<AnnualSalary>>()
            {
                ReturnCode = "A0",
                ReturnMessage = "有資料/寫入成功/執行成功",
                ReturnData = new List<AnnualSalary>() {
                    new AnnualSalary(){ 
                    }
                }
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Model
{
    public class AnnualSalary
    {
        public string Year { get; set; }

        public string SalaryItemCategoryI { get; set; }

        public string SalaryItemCategoryIName { get; set; }

        public string CompanyCode { get; set; }

        public string EmployeeCode { get; set; }

        public string EmployeeID { get; set; }

        public string SalaryItemCategoryIII { get; set; }
        public string SalaryItemCategoryIIIName { get; set; }
        public string PlusorMinus { get; set; }
        public string Salary_Amt { get; set; }
        public string Stock_Amt { get; set; }
        public decimal Stock_Prices { get; set; }
        public string? SalaryItemRemark { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastChgDate { get; set; }

    }
}

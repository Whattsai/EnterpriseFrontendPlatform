using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Model
{
    public class GetBounsAndSalaryResponse
    {
        public string EmployeeID { get; set; }

        public string EmployeeName { get; set; }

        public string JobTitle { get; set; }

        public string BackAccount1 { get; set; }

        public string BackAccount2 { get; set; }

        public List<ERDDDetail> ERDD { get; set; } = new List<ERDDDetail>();

        public int ERTotal { get; set; }

        public int DDTotal { get; set; }

        public IEnumerable<string> SalaryItemRemark { get; set; }

        public IEnumerable<string> Remark { get; set; }
        public int Total { get; set; }
        public int YearSalary { get; set; }
        public List<AnnualSalaryDetail> AnnualSalary { get; set; } = new List<AnnualSalaryDetail>();
    }

    public class ERDDDetail
    {
        public string ERName { get; set; }

        public string ERSalary { get; set; }

        public string DDName { get; set; }

        public string DDSalary { get; set; }
    }

    public class AnnualSalaryDetail
    {
        public string SalaryItemCategoryIIIName { get; set; }

        public string SalaryAmt { get; set; }
    }
}

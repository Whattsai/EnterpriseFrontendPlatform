using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Model.Bounts
{
    public class GetBonusRequest
    {
        public GetBonusRequest(int code, string id, int year, string lang)
        {
            CompanyCode = code;
            EmpID = id;
            Year = year;
            Lang = lang;
        }

        public int CompanyCode { get; set; }

        public string EmpID { get; set; }

        public int Year { get; set; }

        public string? Lang { get; set; }
    }
}

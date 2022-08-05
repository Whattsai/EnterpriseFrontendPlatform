using Aggregate.Model;
using HR.Model;
using SJ.Convert;
using SJ.ObjectMapper.Module;
using System.Collections.Concurrent;

namespace HR.Moudule
{
    public class GetBounsAndSalaryMapper
    {
        Mapper _mapper = new Mapper();

        public GetBounsAndSalaryResponse Go(ConcurrentDictionary<string, StateModel> statModel)
        {
            List<Bouns>? bounsDatas = _mapper.GetDataHierarchy<List<Bouns>>("Action_HRGetBonus.ResponseData.ReturnData", statModel);
            List<AnnualSalary>? salaryData = _mapper.GetDataHierarchy<List<AnnualSalary>>("Action_HRGetAnnualSalary.ResponseData.ReturnData", statModel);

            var erList = bounsDatas?.FindAll(c => c.PlusorMinus == "ER");
            var ddList = bounsDatas?.FindAll(c => c.PlusorMinus == "DD");
            if(erList == null || ddList == null)
            {
                throw new Exception("資料有誤:ER與DD不可為空");
            }

            int index = erList.Count() > ddList.Count() ? erList.Count() : ddList.Count();
            int erTotal = 0;
            int ddTotal = 0;
            int total = 0;

            // Bouns
            List<ERDDDetail> erddList = new List<ERDDDetail>();
            for (int i = 0; i < index; i++)
            {
                erddList.Add(new ERDDDetail());

                // er整理
                if (erList.Count() > i)
                {
                    erddList[i].ERName = erList[i].SalaryItemCategoryIIIName;
                    erddList[i].ERSalary = erList[i].Salary_Amt;
                    erTotal += int.Parse(erList[i].Salary_Amt);
                    total += int.Parse(erList[i].Salary_Amt);
                }

                // dd整理
                if (ddList.Count() > i)
                {
                    erddList[i].DDName = ddList[i].SalaryItemCategoryIIIName;
                    erddList[i].DDSalary = ddList[i].Salary_Amt;
                    ddTotal += int.Parse(ddList[i].Salary_Amt);
                    total -= int.Parse(ddList[i].Salary_Amt);
                }
            }

            // Annual Salary
            List<AnnualSalaryDetail> annualSalaryList = new List<AnnualSalaryDetail>();
            int yearSalary = 0;

            foreach (var item in salaryData)
            {
                annualSalaryList.Add(new AnnualSalaryDetail()
                {
                    SalaryItemCategoryIIIName = item.SalaryItemCategoryIIIName,
                    SalaryAmt = item.Salary_Amt
                });
                if (item.PlusorMinus == "DD")
                {
                    yearSalary -= int.Parse(item.Salary_Amt.Replace(",", ""));
                }
                else
                {
                    var tmp = item.Salary_Amt.Replace(",", "");
                    yearSalary += int.Parse(tmp);
                }
            }


            GetBounsAndSalaryResponse ans = new GetBounsAndSalaryResponse()
            {
                EmployeeID = bounsDatas[0].EmployeeID,
                EmployeeName = bounsDatas[0].EmployeeName,
                JobTitle = bounsDatas[0].JobTitle,
                BankAccount1 = bounsDatas[0].BankAccount1,
                BankAccount2 = bounsDatas[0].BankAccount2,
                ERDD = erddList,
                ERTotal = erTotal,
                DDTotal = ddTotal,
                Total = total,
                SalaryItemRemark = bounsDatas.FindAll(c=>c.SalaryItemRemark != null).Select(c => c.SalaryItemRemark),
                Remark = bounsDatas.FindAll(c => c.Remark != null).Select(c => c.Remark),
                AnnualSalary = annualSalaryList,
                YearSalary = yearSalary
            };

            return ans;
        }
    }
}

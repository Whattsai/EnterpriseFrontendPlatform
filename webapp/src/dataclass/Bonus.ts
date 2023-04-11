import { CompanyInfo } from "./CompanyInfo";
import { ERDD } from "./ERDD";
import { AnnualSalary } from "./AnnualSalary";
export class Bonus {
CompanyCode : number = 0;
EmpID : string = '';
Year : number = 0;
Lang : string = '';
Department : string = '';
Selected : string = '';
CompanyInfo : CompanyInfo[] = new Array<CompanyInfo>();





ERDD : ERDD[] = new Array<ERDD>();
ERTotal : number = 0;
DDTotal : number = 0;
Total : number = 0;
Remark : string[] = new Array<string>();

AnnualSalary : AnnualSalary[] = new Array<AnnualSalary>();
}

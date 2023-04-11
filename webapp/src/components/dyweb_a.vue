<template>

                                <div style="display:none">
                                    <div dyweb-model="number">{{bonus.CompanyCode}}</div>
                                    <div dyweb-model="string">{{bonus.EmpID}}</div>
                                    <div dyweb-model="number">{{bonus.Year}}</div>
                                    <div dyweb-model="string">{{bonus.Lang}}</div>
                                </div>
                                <h2 class="title-header">玉山人園地 - 年終查詢</h2>
                                <div class="contents">
                                    <p>
                                        現況服務部門:
                                        <span id="MainContent_lblEMP_Department" v-on:click="Aggr_Post('hr', 'EFP_GetBonusAndSalary', 'url:CompanyCode:CompanyCode', 'url:EmpID:EmpID', 'url:Year:Year', 'url:Lang:Lang')" dyweb-model="string">{{bonus.Department}}</span>
                                    </p>
                                    <div dyweb-model="string">{{bonus.Selected}}</div>
                                    <p>
                                        發薪單位：
                                        <select v-model="bonus.Selected" style="width:300px;" dyweb-watch="bonus.Selected, Aggr_Post('hr', 'EFP_GetBonusAndSalary', 'url:CompanyCode:CompanyCode', 'url:EmpID:EmpID', 'url:Year:Year', 'url:Lang:Lang')" dyweb-selectinitial="Aggr_Post('hr', 'EFP_GetBonusCompany','url:CompanyCode:CompanyCode', 'url:EmpID:EmpID', 'url:Year:Year', 'url:Lang:Lang'))">
                                            <option dyweb-model="object[]" v-for="item in bonus.CompanyInfo" v-bind:value="item.CompanyTypeCode" v-bind:key="item.CompanyTypeCode" dyweb-vfor-model="number">{{item.CompanyTypeName}}</option>
                                        </select>
                                    </p>
                                    <span class="bounses_title" id="bounses_title">員工資料</span>
                                    <table class="supplycontent-table-data">
                                        <tbody>
                                            <tr>
                                                <th class="colored-bg" style="width: 100px;">員工代號</th>
                                                <th class="colored-bg" style="width: 100px;">姓名</th>
                                                <th class="colored-bg">職稱</th>
                                                <th class="colored-bg">帳號一</th>
                                                <th class="colored-bg">帳號二</th>
                                            </tr>
                                            <tr>
                                                <th style="width: 100px;">
                                                    <span id="MainContent_lblEmpSN">{{bonus.EmployeeID}}</span>
                                                </th>
                                                <th style="width: 100px;">
                                                    <span id="MainContent_lblEmpName">{{bonus.EmployeeName}}</span>
                                                </th>
                                                <th>
                                                    <span id="MainContent_lblEmpTitle">{{bonus.JobTitle}}</span>
                                                </th>
                                                <th>
                                                    <span id="MainContent_lblEmpAccount1">{{bonus.BankAccount1}}</span>
                                                </th>
                                                <th>
                                                    <span id="MainContent_lblEmpAccount2">{{bonus.BankAccount2}}</span>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>
                                    <div id="YearEndBonusContents">
                                        <p id="MainContent_dynamicLabelAdd" class="location"></p>
                                        <div id="MainContent_panelBonus">

                                            <span class="bounses_title" id="bounses_title">獎金項目</span>
                                            <span id="MainContent_lbl_BounsesContent"></span><table class="supplycontent-table-data">
                                                <tbody>
                                                    <tr>
                                                        <th width="25%" class="colored-bg">應發項目</th>
                                                        <th width="25%" class="colored-bg">金額</th>
                                                        <th width="25%" class="colored-bg">應扣項目</th>
                                                        <th width="25%" class="colored-bg">金額</th>
                                                    </tr>
                                                    <tr dyweb-model="object[]" v-for="item in bonus.ERDD">
                                                        <td>{{item.ERName}}</td>
                                                        <td dyweb-vfor-model="number">{{item.ERSalary}}</td>
                                                        <td>{{item.DDName}}</td>
                                                        <td dyweb-vfor-model="number">{{item.DDSalary}}</td>
                                                    </tr>
                                                    <tr>
                                                        <th class="colored-bg">應發數</th>
                                                        <th>
                                                            <span class="th_r">
                                                                <span id="MainContent_lblPaySum" style="color: black; font-weight: 700" dyweb-model="number">{{bonus.ERTotal}}</span>
                                                            </span>
                                                        </th>
                                                        <th class="colored-bg">應扣數</th>
                                                        <th>
                                                            <span class="th_r">
                                                                <span id="MainContent_lblWithHoldSum" style="color: black; font-weight: 700" dyweb-model="number">{{bonus.DDTotal}}</span>
                                                            </span>
                                                        </th>
                                                    </tr>
                                                    <tr class="colored-bg">
                                                        <th class="colored-bg" colspan="4">實發金額</th>
                                                    </tr>
                                                    <tr>
                                                        <th colspan="4">
                                                            <span id="MainContent_lblSum" style="color: black; font-weight: 700" dyweb-model="number">{{bonus.Total}}</span>
                                                        </th>
                                                    </tr>
                                                </tbody>
                                            </table>
                                            <h3 class="title" style="color: #99cc66;">備註</h3>
                                            <span id="MainContent_lb_BonusRemark"></span><table class="table-notice">
                                                <tbody>
                                                    <tr dyweb-model="string[]" v-for="item in bonus.Remark"><th style="color:#99cc66;">※</th><td>{{item}}</td></tr>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                    <span class="bounses_title" id="bounses_title">年度薪酬</span>
                                    <table class="supplycontent-table-data">
                                        <tbody>
                                            <tr>
                                                <th>
                                                    <span id="MainContent_lbYearSalary">{{bonus.YearSalary}}</span>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>

                                    <div id="sShowYearSalary">
                                        <div id="ShowYearS">
                                            <span class="bounses_title" id="bounses_title">2021年度薪酬明細</span>
                                            <span id="MainContent_lbSalarytable">
                                            <table class="supplycontent-table-data">
                                                <tbody>
                                                <tr dyweb-model="object[]" v-for="item in bonus.AnnualSalary"><th class="colored-bg">{{item.SalaryItemCategoryIIIName}}</th><th>{{item.SalaryAmt}}</th></tr>
                                                    </tbody>
                                                </table>
                                            </span>
                                        </div>
                                        <h3 class="title" style="color: #99cc66;">備註</h3>
                                        <span id="MainContent_lb_AnnualSalaryRemark"></span><table class="table-notice">
                                            <tbody>
                                                <tr><th style="color:#99cc66;">※</th><td>彙總範圍含玉山金控及子公司2021年度薪酬資料</td></tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                                <div class="supplycontent-footer-btn">
                                    <a href="https://ehr.esunbank.com.tw/EHR/Profile/ProfileMain.aspx">回上一頁</a>
                                </div>


                            
</template>
<script lang='ts'>
import { watch, reactive, defineComponent } from 'vue';
import 'reflect-metadata';
import 'es6-shim';
import { classToPlain, deserialize, plainToClassFromExist } from 'class-transformer';
import axios from 'axios';
    import { Bonus } from '../dataclass/Bonus';
    import { CompanyInfo } from '../dataclass/CompanyInfo';
    import { ERDD } from '../dataclass/ERDD';
    import { AnnualSalary } from '../dataclass/AnnualSalary';

    export default defineComponent({
setup() {

const bonus = reactive(new Bonus());

            watch(() => bonus.Selected, (newValue, oldValue) => {
                Aggr_Post('hr', 'EFP_GetBonusAndSalary', 'url:CompanyCode:CompanyCode', 'url:EmpID:EmpID', 'url:Year:Year', 'url:Lang:Lang')
});


            const Aggr_Post = (serviceName: string, executeKey: string, ...perameters: string[]) => {
var jsonstring = "{ ";
for (var i = 0; i < perameters.length; i++) {
if (i > 0) { jsonstring += ',' }
if (perameters[i].includes('url:')) {
const paramInfo = perameters[i].replace("url: ", "").split(':');
const urlParams = new URLSearchParams(window.location.search);
const paramValue = urlParams.get(paramInfo[1]);
jsonstring += '"' + paramInfo[1] + '":"' + paramValue + '"';
} else {
const key = perameters[i] as keyof Bonus;
jsonstring += '"' + perameters[i] + '":"' + bonus[key] + '"';
                    }
                }
jsonstring += "} ";
var postData = JSON.parse(jsonstring);
 const automapp = (jsonData: any) => {
 const multiJson = classToPlain(jsonData);
 plainToClassFromExist(bonus, multiJson);
 }
                const postRequest = { Service: serviceName, ID: executeKey, Data: postData }
 axios.post('http://localhost:5002/Aggregate/Go', postRequest)
 .then((response) => automapp(response.data));
}

            Aggr_Post('hr', 'EFP_GetBonusAndSalary', 'url:CompanyCode:CompanyCode', 'url:EmpID:EmpID', 'url:Year:Year', 'url:Lang:Lang')

            Aggr_Post('hr', 'EFP_GetBonusCompany', 'url:CompanyCode:CompanyCode', 'url:EmpID:EmpID', 'url:Year:Year', 'url:Lang:Lang')


return { bonus, Aggr_Post }
}
})
</script>

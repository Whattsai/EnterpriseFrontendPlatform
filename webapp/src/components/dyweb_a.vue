<template>

                                <h2 class="title-header">玉山人園地 - 年終查詢</h2>
                                <div class="contents">
                                    <p>
                                        現況服務部門:
                                        <span id="MainContent_lblEMP_Department" v-on:click="Aggr_Post('bgkey2','url:CompanyID:CompanyID', 'url:EmpID:EmpID', 'url:Year:Year')" dyweb-model="string">{{bonusInfo.Department}}</span>
                                    </p>
                                    <div dyweb-model="string">{{bonusInfo.Selected}}</div>
                                    <p>
                                        發薪單位：
                                        <select v-model="bonusInfo.Selected" style="width:300px;" dyweb-watch="bonusInfo.Selected,Aggr_Post('C','Title')" dyweb-selectinitial="Aggr_Post('selectinitial','Title'))">
                                            <option dyweb-model="object[]" v-for="item in bonusInfo.Companies" :value="item.ID" :key="item.ID" dyweb-vfor-model="number">{{item.ID}}</option>
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
                                                    <span id="MainContent_lblEmpSN" dyweb-model="number">{{bonusInfo.ID}}</span>
                                                </th>
                                                <th style="width: 100px;">
                                                    <span id="MainContent_lblEmpName" dyweb-model="string">{{bonusInfo.Name}}</span>
                                                </th>
                                                <th>
                                                    <span id="MainContent_lblEmpTitle" dyweb-model="boolean">123456798</span>
                                                </th>
                                                <th>
                                                    <span id="MainContent_lblEmpAccount1" dyweb-model="string">223456789</span>
                                                </th>
                                                <th>
                                                    <span id="MainContent_lblEmpAccount2">9876543210</span>
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
                                                    <tr dyweb-model="object[]" v-for="item in bonusInfo.ER">
                                                        <td>{{item.Description}}</td>
                                                        <td dyweb-vfor-model="number">{{item.Amount}}</td>
                                                        <td>獎金補充保費</td>
                                                        <td>1,250</td>
                                                    </tr>
                                                    <tr>
                                                        <th class="colored-bg">應發數</th>
                                                        <th>
                                                            <span class="th_r">
                                                                <span id="MainContent_lblPaySum" style="color: black; font-weight: 700" dyweb-model="number">{{bonusInfo.ERTotal}}</span>
                                                            </span>
                                                        </th>
                                                        <th class="colored-bg">應扣數</th>
                                                        <th>
                                                            <span class="th_r">
                                                                <span id="MainContent_lblWithHoldSum" style="color: black; font-weight: 700" dyweb-model="number">{{bonusInfo.DDTotal}}</span>
                                                            </span>
                                                        </th>
                                                    </tr>
                                                    <tr class="colored-bg">
                                                        <th class="colored-bg" colspan="4">實發金額</th>
                                                    </tr>
                                                    <tr>
                                                        <th colspan="4">
                                                            <span id="MainContent_lblSum" style="color: black; font-weight: 700" dyweb-model="number">{{bonusInfo.Total}}</span>
                                                        </th>
                                                    </tr>
                                                </tbody>
                                            </table>
                                            <h3 class="title" style="color: #99cc66;">備註</h3>
                                            <li dyweb-model="object[]" v-for="item in bonusInfo.Memo">
                                                <a dyweb-vfor-model="string">{{item.SalaryItemCategoryIIIName}}</a>
                                                <a dyweb-vfor-model="string">{{item.SalaryItemRemark}}</a>
                                                <a dyweb-vfor-model="string">{{item.Remark}}</a>
                                            </li>
                                            <span id="MainContent_lb_BonusRemark"></span><table class="table-notice">
                                                <tbody>
                                                    <tr><th style="color:#99cc66;">※</th><td>特別獎金：表彰你對玉山的特別貢獻，謹致謝忱。</td></tr>
                                                    <tr><th style="color:#99cc66">※</th><td>獎金補充保費：全年累計超過健保投保金額4倍部分的獎金扣繳2.11%。</td></tr>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                    <span class="bounses_title" id="bounses_title">年度薪酬</span>
                                    <table class="supplycontent-table-data">
                                        <tbody>
                                            <tr>
                                                <th>
                                                    <span id="MainContent_lbYearSalary">1,000,000</span>
                                                </th>
                                            </tr>
                                        </tbody>
                                    </table>

                                    <div id="sShowYearSalary">
                                        <div id="ShowYearS">
                                            <span class="bounses_title" id="bounses_title">2021年度薪酬明細</span>
                                            <span id="MainContent_lbSalarytable"><table class="supplycontent-table-data"><tbody><tr><th class="colored-bg">固定薪</th><th>500,000</th></tr><tr><th class="colored-bg">年終獎金</th><th>100,000</th></tr><tr><th class="colored-bg">員工儲蓄會獎勵儲金</th><th>24,000</th></tr><tr><th class="colored-bg">其他薪資</th><th>4,500</th></tr><tr><th class="colored-bg">加值班費</th><th>1000</th></tr><tr><th class="colored-bg">員工酬勞</th><th>100,000</th></tr><tr><th class="colored-bg">年節福利金</th><th>15,000</th></tr></tbody></table></span>
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

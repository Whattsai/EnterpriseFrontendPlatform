<template>
    <img alt="Vue logo" src="./assets/logo.png">
    <HelloWorld msg="Welcome to Your Vue.js + TypeScript App" />
    <div id="App" oa-initail="true" oa-source="info" oa-post="http://abcde{empNo}">
        <section class="content-wrapper main-content clear-fix">

            <div class="wrap supplycontent">
                <div class="sub-wrap sub-wrap-shadow sub-wrap-color">
                    <div class="container">
                        <div class="info">
                            <h2 class="title-header">{{ Title }}</h2>
                            <div class="contents">
                                <p>
                                    現況服務部門:
                                    <span id="MainContent_lbl">{{ EMP_Department }}</span>
                                </p>
                                <p>
                                    發薪單位：
                                    <!--下拉選單-->
                                    <select name="ctl00$MainContent$ddl_Company" id="MainContent_ddl_Company" style="width:300px;">
                                        <option selected="selected" value="001">玉山商業銀行股份有限公司</option>
                                    </select>
                                </p>
                                <span class="bounses_title" id="bounses_title">員工資料</span>
                                <!--TableForeach-->
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
                                                <span id="MainContent_lblEmpSN">{{EMP_No}}</span>
                                            </th>
                                            <th style="width: 100px;">
                                                <span id="MainContent_lblEmpName">OOO</span>
                                            </th>
                                            <th>
                                                <span id="MainContent_lblEmpTitle">工程師</span>
                                            </th>
                                            <th>
                                                <span id="MainContent_lblEmpAccount1">1234567890</span>
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
                                                <tr><td>特別獎金</td><td>100,000</td><td>獎金補充保費</td><td>1,250</td></tr>
                                                <tr><td>年終獎金</td><td>100,000</td><td>所得稅</td><td>9,000</td></tr>
                                                <tr><td>春節福利金</td><td>5,000</td><td></td><td></td></tr>
                                                <tr>
                                                    <th class="colored-bg">應發數</th>
                                                    <th>
                                                        <span class="th_r">
                                                            <span id="MainContent_lblPaySum" style="color: black; font-weight: 700">200,000</span>
                                                        </span>
                                                    </th>
                                                    <th class="colored-bg">應扣數</th>
                                                    <th>
                                                        <span class="th_r">
                                                            <span id="MainContent_lblWithHoldSum" style="color: black; font-weight: 700">9,999</span>
                                                        </span>
                                                    </th>
                                                </tr>
                                                <tr class="colored-bg">
                                                    <th class="colored-bg" colspan="4">實發金額</th>
                                                </tr>
                                                <tr>
                                                    <th colspan="4">
                                                        <span id="MainContent_lblSum" style="color: black; font-weight: 700">200,000</span>
                                                    </th>
                                                </tr>
                                            </tbody>
                                        </table>
                                        <h3 class="title" style="color: #99cc66;">備註</h3>
                                        <span id="MainContent_lb_BonusRemark"></span><table class="table-notice">
                                            <tbody>
                                                <tr><th style="color:#99cc66">※</th><td>特別獎金：表彰你對玉山的特別貢獻，謹致謝忱。</td></tr>
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
                        </div>
                    </div>
                </div>
            </div>
            <!-- End page content -->
            <!-- Begin footer -->
            <div class="footer">
                <div class="container">

                    <h2>玉山銀行人力資源處服務專線</h2>
                    <h4 style="font-weight:100;font-size:22px;">(02)2175-1313分機8701</h4>
                    <br>

                    <a href="http://www.esunbank.com.tw/" style="font-weight:600;font-family:Microsoft JhengHei, eudc">玉山銀行  </a>
                    <a href="https://ehrd.esunbank.com.tw/eHRD/" style="font-weight:100;font-size:22px;">  E-learning  </a>
                    <span class="copyright">Copyright © 2015 E.SUN Commercial Bank, Ltd. All Rights Reserved.</span>
                </div>
            </div>



        </section>
    </div>
</template>

<script lang="ts">
    // import { Options, Vue } from 'vue-class-component';
    import HelloWorld from './components/HelloWorld.vue';
    import './css/style.css';
    import './css/YearEndBonusesreset.css';
    import './css/YearEndBonusesstyle.css';

    export default {
        name: 'App',
        components: {
            HelloWorld
        },
        data() {
            return {
                Title: "年終獎金查詢",
                EMP_Department: "資訊處 後勤資訊發展中心 E32-OA2",
                EMP_No: "00000",
            };
        },
        mounted:
        {

        },
        methods: {
            initail() {

            },
            post() {

            },
            get() {

            },
            click1() {

            },
            click2() {

            }
        }
    }
    //@Options({
    //    components: {
    //        HelloWorld,
    //    },
    //})
    //export default class App extends Vue {
    //    data() {
    //        return {
    //            title: "年終獎金查詢",
    //            EMP_Department: "資訊處 後勤資訊發展中心 E32-OA2",
    //            count: 6
    //        };
    //    }
    //}
</script>

<style>
    #app {
        font-family: Avenir, Helvetica, Arial, sans-serif;
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
        text-align: center;
        color: #2c3e50;
        margin-top: 60px;
    }
</style>

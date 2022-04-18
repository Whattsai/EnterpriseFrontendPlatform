<template>
    <div oa-initail="oaPost(1,'method1',url:empID, empNo)">
        <section class="content-wrapper main-content clear-fix">

            <div class="wrap supplycontent">
                <div class="sub-wrap sub-wrap-shadow sub-wrap-color">
                    <div class="container">
                        <div class="info">
                            <h2 class="title-header">{{ Person.Title }}</h2>
                            <div class="contents">
                                <p>
                                    現況服務部門:
                                    <span id="MainContent_lbl">{{ Person.EMP_Department }}</span>
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
                                            <th class="colored-bg" style="width: 100px;">員 工代號</th>
                                            <th class="colored-bg" style="width: 100px;">姓名</th>
                                            <th class="colored-bg">職稱</th>
                                            <th class="colored-bg">帳號一</th>
                                            <th class="colored-bg">帳號二</th>
                                        </tr>
                                        <tr>
                                            <th style="width: 100px;">
                                                <span id="MainContent_lblEmpSN">{{Person.EMP_No}}</span>
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
                                <span class="bounses_title" id="bounses_title" v-on:click="automapp()">年度薪酬</span>
                                <table class="supplycontent-table-data">
                                    <tbody>
                                        <tr>
                                            <th>
                                                <span id="MainContent_lbYearSalary">1,000,000</span>
                                            </th>
                                        </tr>
                                    </tbody>
                                </table>

                                <div id="sShowYearSalary" v-on:click="oapost('bgkey','Title','EMP_Department')">
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
    //***必要import***
      //import moment from 'moment'
    import { Vue } from 'vue-class-component';
    import 'reflect-metadata';
    import 'es6-shim';
    import { classToPlain, deserialize, plainToClassFromExist } from "class-transformer";
    import axios from 'axios';

    export default class HelloWorld extends Vue {

        Person: Person = new Person();

        mounted() {
            console.log('mounted');
            this.Aggr_Post("A", "Title", "ID");
        }

        initail() {
            //依照OA-Iinitial順序排列
            this.Aggr_Post("Key1", "A", "B", "C", "D", "E");
            this.Aggr_Post("Key2", "XXX", "OOO");
        }

        method1(...perameters: string[]) {
            for (var i = 0; i < perameters.length; i++) {
                console.log(perameters[i]);
            }
        }

        //OA預設方法
        Aggr_Post(executeKey: string, ...perameters: string[]) {
            var jsonstring = "{";
            for (var i = 0; i < perameters.length; i++) {
                if (i > 0) { jsonstring += ',' }
                const key = perameters[i] as keyof Person;
                //依照參數組PostData Json字串再轉object
                jsonstring += '"' + perameters[i] + '":"' + this.Person[key] + '"';
                console.log(perameters[i] + ":" + this.Person[key]);
            }
            jsonstring += "}"
            console.log(jsonstring);
            var postData = JSON.parse(jsonstring);

            console.log(postData);

            axios({
                method: 'post',
                url: 'https://hexschool-tutorial.herokuapp.com/api/signup',
                //API要求的資料
                data: postData
            }).then((response) => this.auto_mapp(response.data))
        }

        auto_mapp(jsonData: string) {
            let p = deserialize(Person, jsonData);
            const multiJson = classToPlain(p);
            plainToClassFromExist(this.Person, multiJson);
        }
    }

    class Person {
        Title: string = "年終獎金查詢";
        ID: string = "1234567";
        EMP_Department: string = "資訊處 後勤資訊發展中心 E32-OA2";
        EMP_No: string = "00000";
        A: A = new A();
        Aarray: A[] = new Array<A>();
        StringArray: string[] = new Array<string>();
        NumberArray: number[] = new Array<number>();
        B: boolean = false;
    }

    class A {
        X: string = '';
        Y: string = '';
    }
</script>


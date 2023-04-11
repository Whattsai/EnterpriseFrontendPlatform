<template>

                                <div dyweb-model="string">{{secondModel.Selected}}</div>
                                <select v-model="secondModel.Selected" style="width:300px;" dyweb-watch="secondModel.Selected,Aggr_Post('C','Title')" dyweb-selectinitial="Aggr_Post('selectinitial','Title'))">
                                    <option dyweb-model="object[]" v-for="item in secondModel.CompaniesB" :value="item.ID" :key="item.ID" dyweb-vfor-model="string">{{item.Name}}</option>
                                </select>
                            
</template>
<script lang='ts'>
import { watch, reactive, defineComponent } from 'vue';
import 'reflect-metadata';
import 'es6-shim';
import { classToPlain, deserialize, plainToClassFromExist } from 'class-transformer';
import axios from 'axios';
import {SecondModel} from '../dataclass/SecondModel';
import {CompaniesB} from '../dataclass/CompaniesB';

export default defineComponent ({
setup() {

const secondModel = reactive(new SecondModel());

watch(() =>secondModel.Selected, (newValue, oldValue) => {
Aggr_Post('C','Title')
});


const Aggr_Post = (serviceName:string, executeKey: string, ...perameters: string[]) => {
var jsonstring = "{ ";
for (var i = 0; i < perameters.length; i++) {
if (i > 0) { jsonstring += ',' }
if (perameters[i].includes('url:')) {
const paramInfo = perameters[i].replace("url: ", "").split(':');
const urlParams = new URLSearchParams(window.location.search);
const paramValue = urlParams.get(paramInfo[1]);
jsonstring += '"' + paramInfo[1] + '":"' + paramValue + '"';
} else {
const key = perameters[i] as keyof SecondModel;
jsonstring += '"' + perameters[i] + '":"' + secondModel[key] + '"';
}}
jsonstring += "} ";
var postData = JSON.parse(jsonstring);
 const automapp = (jsonData: any) => {
 const multiJson = classToPlain(jsonData);
 plainToClassFromExist(secondModel, multiJson);
 }
 const postRequest = {Service: serviceName, ID: executeKey, Data: postData }
 axios.post('http://localhost:5002/Aggregate/Go', postRequest)
 .then((response) => automapp(response.data));
}

Aggr_Post('OtherPage','bgKey', 'url:CompanyID:CompanyID',  'url:ID:ID')

Aggr_Post('selectinitial','Title')


return { secondModel, Aggr_Post }
}
})
</script>

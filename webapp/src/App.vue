<template>

                    <div class="sub-wrap sub-wrap-shadow sub-wrap-color">
                        <div class="container">
                            <dyweb_a></dyweb_a>

                            <!--<div class="info" dyweb-initial="Aggr_Post('bgKey','url:CompanyID:CompanyID', 'url:ID:ID')">
                                <div dyweb-model="string">{{secondModel.Selected}}</div>
                                <select v-model="secondModel.Selected" style="width:300px;" dyweb-watch="secondModel.Selected,Aggr_Post('C','Title')" dyweb-selectinitial="Aggr_Post('selectinitial','Title'))">
                                    <option dyweb-model="object[]" v-for="item in secondModel.CompaniesB" :value="item.ID" :key="item.ID" dyweb-vfor-model="string">{{item.Name}}</option>
                                </select>
                            </div>-->
                        </div>
                    </div>
                
</template>
<script lang='ts'>
import { defineComponent } from 'vue';
import dyweb_a from './components/dyweb_a.vue';
export default defineComponent({
name: 'App',
components: {
"dyweb_a": dyweb_a,
 },
})
</script>

<template>

                                <div dyweb-model="string">{{secondModel.Selected}}</div>
                                <select v-model="secondModel.Selected" style="width:300px;" dyweb-watch="secondModel.Selected,Aggr_Post('C','Title')" dyweb-selectinitial="Aggr_Post('selectinitial','Title'))">
                                    <option dyweb-model="object[]" v-for="item in secondModel.CompaniesB" :value="item.ID" :key="item.ID" dyweb-vfor-model="string">{{item.Name}}</option>
                                </select>
                            
</template>

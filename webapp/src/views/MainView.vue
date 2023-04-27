<template>

                    <router-view></router-view>

                            <div class="sub-wrap sub-wrap-shadow sub-wrap-color">
                                <div class="container">
                                    <yearendbonusescomponent></yearendbonusescomponent>

                                    <othercomponent></othercomponent>
                                </div>
                            </div>
                        
</template>
<script lang='ts'>
import { defineComponent } from 'vue';
import DetailViewComponent from './components/DetailViewComponent.vue';
import YearEndBonusesComponent from './components/YearEndBonusesComponent.vue';
import OtherComponent from './components/OtherComponent.vue';
export default defineComponent({
name: 'App',
components: {
"DetailViewComponent": DetailViewComponent,
"YearEndBonusesComponent": YearEndBonusesComponent,
"OtherComponent": OtherComponent,
 },
})
</script>

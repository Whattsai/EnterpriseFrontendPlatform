<template>

                        <div>AAA</div>
                        <detailviewcomponent></detailviewcomponent>
                    
</template>
<script lang='ts'>
import { defineComponent } from 'vue';
import DetailViewComponent from './components/DetailViewComponent.vue';
export default defineComponent({
name: 'App',
components: {
"DetailViewComponent": DetailViewComponent,
 },
})
</script>

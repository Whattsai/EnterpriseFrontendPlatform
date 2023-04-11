<template>

                    <div class="sub-wrap sub-wrap-shadow sub-wrap-color">
                        <div class="container">
                            <yearendbonuses></yearendbonuses>

                            <otherpage></otherpage>
                        </div>
                    </div>
                
</template>
<script lang='ts'>
import { defineComponent } from 'vue';
import YearEndBonuses from './components/YearEndBonuses.vue';
import OtherPage from './components/OtherPage.vue';
export default defineComponent({
name: 'App',
components: {
"YearEndBonuses": YearEndBonuses,
"OtherPage": OtherPage,
 },
})
</script>

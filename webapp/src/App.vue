<template>

        <div class="sub-wrap sub-wrap-shadow sub-wrap-color">
            <div class="container">
                <dyweb_a></dyweb_a>
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

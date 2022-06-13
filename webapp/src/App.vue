<template>

        <div class="sub-wrap sub-wrap-shadow sub-wrap-color">
            <div class="container">
                <dyweb_a></dyweb_a>

                <dyweb_b></dyweb_b>
            </div>
        </div>

</template>
<script lang='ts'>
import { defineComponent } from 'vue';
import dyweb_a from './components/dyweb_a.vue';
import dyweb_b from './components/dyweb_b.vue';
export default defineComponent({
name: 'App',
components: {
"dyweb_a": dyweb_a,
"dyweb_b": dyweb_b,
 },
})
</script>

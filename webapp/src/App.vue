<template>
    <img alt="Vue logo" src="./assets/logo.png">
    <HelloWorld msg="Welcome to Your Vue.js + TypeScript App" />
</template>



<script lang="ts">
    //***必要import***
    import { Options, Vue } from 'vue-class-component';

    //***vue***
    import HelloWorld from './components/HelloWorld.vue';

    //***css***
    import './css/style.css';
    import './css/YearEndBonusesreset.css';
    import './css//YearEndBonusesstyle.css';

    @Options({
        components: {
            HelloWorld,
        },
    })
    export default class App extends Vue {

    }
</script>


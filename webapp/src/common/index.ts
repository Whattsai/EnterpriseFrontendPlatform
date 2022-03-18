import { createApp } from "vue";
import App from "../app-components/App.vue";

const app = createApp({
    data() {
        return {
            message: 'Hello Vue!'
        }
    }
}).mount('#app');

function TSButton() {
    let name: string = "Fred";
    document.getElementById("ts-example").innerHTML = "abcde";
}
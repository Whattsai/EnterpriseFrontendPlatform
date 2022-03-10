"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const vue_1 = require("vue");
const app = (0, vue_1.createApp)({
    data() {
        return {
            message: 'Hello Vue!'
        };
    }
}).mount('#app');
function TSButton() {
    let name = "Fred";
    document.getElementById("ts-example").innerHTML = "abcde";
}

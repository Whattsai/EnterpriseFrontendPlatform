"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const HelloWorld_vue_1 = __importDefault(require("./HelloWorld.vue"));
exports.default = {
    name: 'App',
    components: {
        HelloWorld: HelloWorld_vue_1.default
    }
};

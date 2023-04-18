import { createRouter, createWebHistory, RouteRecordRaw } from 'vue-router';
import MainView from '../views/MainView.vue';
import DetailView from '../views/DetailView.vue';
import NotFound from '../views/NotFound.vue';
import axios from 'axios';

const routes: Array<RouteRecordRaw> = [
    {
        path: '/',
        name: 'MainView',
        component: () => import('../views/MainView.vue'),
        children: [
            { name: 'DetailView', path: 'DetailView', component: DetailView, },
        ],
    },
    {
        path: '/:pathMatch(.*)*',
        name: 'NotFound',
        component: NotFound,
    },
]
const router = createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes
})
export default router

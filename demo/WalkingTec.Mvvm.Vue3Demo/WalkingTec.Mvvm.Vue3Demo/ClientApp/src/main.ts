import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import App from './App.vue'
import client from './client/use'
import use from './components/use'
import globalProperties from './globalProperties'
import router from './router'
console.log("LENG ~ router", router)
createApp(App).use(use).use(client).use(createRouter({
    history: createWebHistory(process.env.BASE_URL),
    routes: router.Routers
})).use(globalProperties).mount('#app')

import { createApp } from 'vue'
import App from './App.vue'
import './assets/styles/index.less'
import client from './client/use'
import components from './components/use'
import router from './router'
const RootApp = createApp(App)
    .use(components)
    .use(client)
    .use(router.Router);
RootApp.mount('#app')
RootApp.config.performance = false

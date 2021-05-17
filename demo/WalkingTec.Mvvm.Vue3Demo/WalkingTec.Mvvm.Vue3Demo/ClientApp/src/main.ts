import 'nprogress/nprogress.css';
import { createApp } from 'vue';
import App from './App.vue';
import './assets/styles/index.less';
import client from './client/use';
import components from './components/use';
import AppRouter from './router';
AppRouter.onInit()
const RootApp = createApp(App)
    .use(components)
    .use(client)
    .use(AppRouter.Router);
RootApp.mount('#app')
RootApp.config.performance = false

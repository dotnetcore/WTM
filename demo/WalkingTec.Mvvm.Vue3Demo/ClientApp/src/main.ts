if (import.meta.env.PROD == true) {
  console.log = function () {}
}

import { createApp } from 'vue';
import pinia from '/@/stores/index';
import App from './App.vue';
import router from './router';
import { directive } from '/@/directive/index';
import { i18n } from '/@/i18n/index';
import other from '/@/utils/other';
import ElementPlus from 'element-plus';
import 'element-plus/dist/index.css';
import '/@/theme/index.scss';
import VueGridLayout from 'vue-grid-layout';
import wtm from '/@/components/index'
const app = createApp(App);
directive(app);
other.elSvg(app);
app.use(pinia).use(router).use(ElementPlus).use(i18n).use(wtm).use(VueGridLayout).mount('#app');

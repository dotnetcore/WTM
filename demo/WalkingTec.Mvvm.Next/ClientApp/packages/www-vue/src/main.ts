import Antd from 'ant-design-vue';
import 'ant-design-vue/dist/antd.less';
import './styles/base.less';
import 'viewerjs/dist/viewer.css';
import 'nprogress/nprogress.css';
import Vue from 'vue';
import router from './router';
import VueI18n from 'vue-i18n';
import locale, { createVueI18n } from './locale';
import App from './app.vue';
import Globalconfig from './global.config';
import BasicData from './basicData';
import wtm from './components';
wtm.install(Vue);
Vue.use(VueI18n)
Antd.install(Vue);
Vue.config.productionTip = false;
Vue.prototype.$GlobalConfig = Globalconfig;
Vue.prototype.$BasicData = BasicData;
Globalconfig.hydrate(Globalconfig.settings.title, Globalconfig)
  // post hydration
  .then(() => {
    console.warn("TCL: Globalconfig", Globalconfig)
    const i18n = createVueI18n();
    const Root = new Vue({
      i18n,
      router,
      // render: h => h('router-view')
      render: h => h(App)
    });
    Root.$mount('#app');
  })


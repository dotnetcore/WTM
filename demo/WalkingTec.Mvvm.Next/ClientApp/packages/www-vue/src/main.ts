import Antd from 'ant-design-vue';
import 'ant-design-vue/dist/antd.less';
import './styles/base.less';
import 'viewerjs/dist/viewer.css';
import 'nprogress/nprogress.css';
import Vue from 'vue';
import router from './router';
import VueI18n from 'vue-i18n';
import locale from './locale';
import App from './app.vue';
import globalconfig from './global.config';
import wtm from './components';
wtm.install(Vue);
Vue.use(VueI18n)
Antd.install(Vue);
Vue.config.productionTip = false
const i18n = new VueI18n({
  locale: globalconfig.settings.language, // set locale en-US zh-CN
  messages: locale
});
const Root = new Vue({
  i18n,
  router,
  // render: h => h('router-view')
  render: h => h(App)
});
Root.$mount('#app');

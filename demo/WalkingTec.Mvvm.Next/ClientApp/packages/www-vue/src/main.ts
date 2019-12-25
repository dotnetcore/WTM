import Antd from 'ant-design-vue';
import 'ant-design-vue/dist/antd.less';
import './styles/base.less';
import 'nprogress/nprogress.css';
import Vue from 'vue';
import router from './router';
import VueI18n from 'vue-i18n';
import locale from './locale';
Vue.use(VueI18n)
Antd.install(Vue);
Vue.config.productionTip = false
const i18n = new VueI18n({
  locale: 'en-US', // set locale en-US zh-CN
  messages: locale
});
const Root = new Vue({
  i18n,
  router,
  render: h => h('router-view')
});
Root.$mount('#app');

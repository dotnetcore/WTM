import Antd from 'ant-design-vue';
import 'ant-design-vue/dist/antd.less';
import './styles/base.less';
import 'nprogress/nprogress.css';
import Vue from 'vue';
import router from './router';
Antd.install(Vue);
Vue.config.productionTip = false
new Vue({
  router,
  render: h => h('router-view')
}).$mount('#app')

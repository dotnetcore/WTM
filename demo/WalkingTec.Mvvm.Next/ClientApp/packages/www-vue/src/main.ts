import Vue from 'vue'
import App from './App.vue'
import router from './router'
import Antd from 'ant-design-vue';
import 'nprogress/nprogress.css';
import 'ant-design-vue/dist/antd.less';
import lodash from 'lodash';
Antd.install(Vue);
Vue.config.productionTip = false
new Vue({
  router,
  render: h => h(App)
}).$mount('#app')
console.log('vue测试', lodash)

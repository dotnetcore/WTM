import Vue from "vue";
import ElementUI from "element-ui";
import i18n from "@/lang";
import SvgIcon from "vue-svgicon";
import App from "@/pages/index.vue";
import router from "@/router";
import store from "@/store/modules/index";
import "@/assets/icon/components";
import "@/router/permission";
import { AppModule } from "@/store/modules/app";
import directives from "@/vue-custom/directive/index";
import filters from "@/vue-custom/filters/index";
import component from "@/vue-custom/component/index";
import prototypes from "@/vue-custom/prototype/index";
import "@/assets/css/index.less";
import "chartist/dist/chartist.min.css";
import "element-ui/lib/theme-chalk/index.css";

Vue.use(ElementUI, {
  size: AppModule.size, // config.elSize, // Set element-ui default size
  i18n: (key: string, value: string) => i18n.t(key, value)
});

Vue.use(SvgIcon, {
  tagName: "svg-icon",
  defaultWidth: "1em",
  defaultHeight: "1em"
});

// 指令
directives.forEach(item => {
  Vue.directive(item.key, item.value);
});
// 过滤器
filters.forEach(item => {
  Vue.filter(item.key, item.value);
});
// 组件
component.forEach(item => {
  Vue.component(_.kebabCase(item.key), item.value);
});



Vue.config.productionTip = false;

new Vue({
  router,
  store,
  i18n,
  render: h => h(App),
  beforeCreate: function() {
    /**
     * prototype
     */
    prototypes.forEach(item => {
        if (_.isFunction(item.value)) {
            Vue.prototype["$" + item.key] = function() {
                return item.value.apply(this, arguments);
            };
        } else {
            Vue.prototype["$" + item.key] = item.value;
        }
    });
  }
}).$mount("#App");

import Vue, { DirectiveOptions, ComputedOptions } from "vue";
import ElementUI from "element-ui";
import config from "@/config/index";
import i18n from "@/lang";
import "element-ui/lib/theme-chalk/index.css";
import "@/assets/css/index.less";
import SvgIcon from "vue-svgicon";
import App from "@/views/index.vue";
import router from "@/router";
import store from "@/store/modules";
import "@/assets/icon/components";
import "@/router/permission";
import { AppModule } from "@/store/modules/app";

import * as directives from "@/util/directive/index";
import * as filters from "@/util/filters/index";
import * as component from "@/util/component/index";
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
Object.keys(directives).forEach(key => {
    Vue.directive(
        key,
        (directives as { [key: string]: DirectiveOptions })[key]
    );
});
// 过滤器
Object.keys(filters).forEach(key => {
    Vue.filter(key, (filters as { [key: string]: Function })[key]);
});
// 组件
Object.keys(component).forEach(key => {
    Vue.component(_.kebabCase(key), component[key]);
});

Vue.config.productionTip = false;

new Vue({
    router,
    store,
    i18n,
    render: h => h(App)
}).$mount("#App");

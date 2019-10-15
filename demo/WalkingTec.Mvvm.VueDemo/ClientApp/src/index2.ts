import Vue from "vue";

import ElementUI from "element-ui";
import config from "@/config/index";
import i18n from "@/lang";

import "element-ui/lib/theme-chalk/index.css";
import "@/assets/css/index.less";

import SvgIcon from "vue-svgicon";
import App from "@/views/index2.vue";
import router from "@/router2";
import store from "@/store/modules";
import "@/assets/icon/components";
import "@/router2/permission";

Vue.use(ElementUI, {
    size: config.elSize, // Set element-ui default size
    i18n: (key: string, value: string) => i18n.t(key, value)
});

Vue.use(SvgIcon, {
    tagName: "svg-icon",
    defaultWidth: "1em",
    defaultHeight: "1em"
});

Vue.config.productionTip = false;

new Vue({
    router,
    store,
    i18n,
    render: h => h(App)
}).$mount("#app");

import Vue from "vue";
import createRouter from "@/router/index";
import store from "@/store/index";
import App from "@/views/index.vue";
import { dirEdit, visible } from "@/util/directive/index";
import "font-awesome/css/font-awesome.min.css";
import "element-ui/lib/theme-chalk/index.css";
import "@/assets/css/index.less";
// import "@/assets/theme/index.css";
import date from "@/util/date.js";
// 饿了吗ui
import ElementUI from "element-ui";
import NProgress from "vue-nprogress";
Vue.use(NProgress, {});
Vue.use(ElementUI);
// 时间格式
Vue.filter(
    "formatTime",
    (value, customFormat = "yyyy-MM-dd hh:mm:ss", isMsec = true) => {
        // customFormat 要展示的时间格式
        // isMsec----传入的value值是否是毫秒
        value = isMsec ? value : value * 1000;
        return date.toFormat(value, customFormat);
    }
);
const nprogress = new NProgress({ parent: ".app-nprogress" });
createRouter().then(router => {
    // 指令
    Vue.directive("edit", dirEdit);
    Vue.directive("visible", visible);
    const app = new Vue({
        nprogress,
        router,
        store,
        render(h) {
            return h(App, {
                props: {
                    projectName: "wtm"
                }
            });
        }
    });
    app.$mount("#App");
});

import Vue from "vue";
import createRouter from "@/router/index";
import store from "@/store/index";
import App from "@/views/index.vue";
import { dirEdit, visible, error } from "@/util/directive/index";
import { formatTime } from "@/util/filters/index";
import { Card, DialogBox } from "@/util/component/index";
import config from "@/config/index";

import "element-ui/lib/theme-chalk/index.css";
import "@/assets/css/index.less";
// 饿了吗ui
import ElementUI from "element-ui";
import NProgress from "vue-nprogress";
Vue.use(NProgress, {});
Vue.use(ElementUI, { size: config.elSize });

const nprogress = new NProgress({ parent: ".app-nprogress" });

createRouter().then(router => {
    // 指令
    Vue.directive("edit", dirEdit);
    Vue.directive("visible", visible);
    Vue.directive("error", error);

    // 过滤器
    Vue.filter("formatTime", formatTime);
    // 组件
    Vue.component("card", Card);
    Vue.component("dialog-box", DialogBox);
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

import Vue from "vue";
import store from "@/store/login/index";
import App from "@/pages/login/app.vue";
// 饿了吗ui
import ElementUI from "element-ui";
if (process.env.LOACL === "loacl") {
    require("element-ui/lib/theme-chalk/index.css");
}
Vue.use(ElementUI);
/* eslint-disable */
const app = new Vue({
    store,
    render(h) {
        return h(App, {
            props: {
                projectName: "login"
            }
        });
    }
});

app.$mount("#App");

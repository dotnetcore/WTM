import Vue from "vue";
import router from "@/router/index";
import store from "@/store/index";
import App from "@/pages/index/app.vue";
import "@/assets/css/index.less";
// import "babel-polyfill";
// 饿了吗ui
import ElementUI from "element-ui";
// if (process.env.LOACL === "loacl") {
//     require("element-ui/lib/theme-chalk/index.css");
// }

Vue.use(ElementUI);
const app = new Vue({
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

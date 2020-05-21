import Vue from "vue";
import store from "@/store/login/index";
import App from "@/pages/login.vue";
import i18n from "@/lang";
// 饿了吗ui
import ElementUI from "element-ui";
require("element-ui/lib/theme-chalk/index.css");
// if (process.env.LOACL === "loacl") {}
Vue.use(ElementUI);
/* eslint-disable */
const app = new Vue({
  store,
  i18n,
  render(h) {
    return h(App, {
      props: {
        projectName: "login"
      }
    });
  }
});

app.$mount("#App");

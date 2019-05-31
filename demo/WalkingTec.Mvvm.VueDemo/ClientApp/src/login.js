import Vue from "vue";
import store from "@/store/login/index";
import App from "@/pages/login/app.vue";

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

import login from "./login/index.vue";
import home from "./Home.vue";
export const Basics = {
    login,
    home
};
export default {
    // login,
    // home,
    user: () => import("./user/index.vue"),
    test: () => import("./test/index.vue"),
}
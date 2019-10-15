import NProgress from "nprogress";
import "nprogress/nprogress.css";
import { Message } from "element-ui";
import { Route } from "vue-router";
import { UserModule } from "@/store/modules/user";
import { PermissionModule } from "@/store/modules/permission";
import i18n from "@/lang"; // Internationalization
import settings from "../settings";
import router from "./index";

NProgress.configure({ showSpinner: false });

const whiteList = ["/login", "/auth-redirect"];

const getPageTitle = (key: string) => {
    const hasKey = i18n.te(`route.${key}`);
    if (hasKey) {
        const pageName = i18n.t(`route.${key}`);
        return `${pageName} - ${settings.title}`;
    }
    return `${settings.title}`;
};

router.beforeEach(async (to: Route, _: Route, next: any) => {
    NProgress.start();
    if (UserModule.token) {
        // Check whether the user has obtained his permission roles
        if (UserModule.roles.length === 0) {
            try {
                // await UserModule.GetUserInfo();
                const roles = ["admin"]; // UserModule.roles;
                PermissionModule.GenerateRoutes(roles);
                //router.addRoutes(PermissionModule.dynamicRoutes);
                console.log("to", to);
                console.log(PermissionModule.dynamicRoutes);
                // next({ ...to, replace: true });
                next();
            } catch (err) {
                // Remove token and redirect to login page
                UserModule.ResetToken();
                Message.error(err || "Has Error");
                location.href = "/login.html";
                // next(`/login?redirect=${to.path}`);
                NProgress.done();
            }
        } else {
            next();
        }
    } else {
        // Has no token
        if (whiteList.indexOf(to.path) !== -1) {
            next();
        } else {
            // Other pages that do not have permission to access are redirected to the login page.
            // next(`/login?redirect=${to.path}`);
            location.href = `/login?redirect=${to.path}`;
            NProgress.done();
        }
    }
});

router.afterEach((to: Route) => {
    // Finish progress bar
    NProgress.done();
    // set page title
    document.title = getPageTitle(to.meta.title);
});

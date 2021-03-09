import NProgress from "nprogress";
import "nprogress/nprogress.css";
import { Message } from "element-ui";
import { Route } from "vue-router";
import { UserModule } from "@/store/modules/user";
import { RoutesModule } from "@/store/modules/routes";
import i18n from "@/lang"; // Internationalization
import settings from "../settings";
import router from "./index";
import pageList from "@/pages/index";

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

const bindLang = ({zh, en}) => {
  const localKey = Object.keys(zh).length > 0 ? Object.keys(zh)[0] : "";
  if (localKey && !i18n.getLocaleMessage('en')[localKey]) {
      i18n.mergeLocaleMessage("en", en);
      i18n.mergeLocaleMessage("zh", zh);
    }
}

router.beforeEach(async (to: Route, _: Route, next: any) => {
  // 路由
  const loadI18n = (isFirst: Boolean = false) => {
    // 加载语言
    import(`@/pages${to.path}/local.ts`).then(LOCAL => {
        bindLang(LOCAL.default)
        isFirst ? next({ ...to, replace: true }) : next();
    }).catch(() => {
        console.warn('找不到 多语言文件');
        isFirst ? next({ ...to, replace: true }) : next();
    })
  }
  NProgress.start();
  if (UserModule.token) {
    // Check roles
    if (UserModule.roles.length === 0) {
      try {
        await UserModule.GetUserInfo();
        RoutesModule.PageList(pageList);
        RoutesModule.GenerateRoutes(UserModule.menus);
        router.addRoutes(RoutesModule.dynamicRoutes);
        loadI18n(UserModule.roles.length > 0);
      } catch (err) {
        Message.error(err || "Has Error");
        location.href = "/login.html";
        // next(`/login?redirect=${to.path}`);
        NProgress.done();
      }
    } else {
      if (to.matched.length !== 0) {
        loadI18n();
      } else {
        next("/404");
      }
    }
  } else {
    // Has no token
    if (whiteList.indexOf(to.path) !== -1) {
      loadI18n();
    } else {
      // next(`/login?redirect=${to.path}`);
      location.href = `/login.html?redirect=${to.path}`;
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



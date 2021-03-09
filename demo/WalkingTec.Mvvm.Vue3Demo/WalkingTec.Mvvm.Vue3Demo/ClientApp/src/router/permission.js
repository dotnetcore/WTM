import { __assign, __awaiter, __generator } from "tslib";
import NProgress from "nprogress";
import "nprogress/nprogress.css";
import { Message } from "element-ui";
import { UserModule } from "@/store/modules/user";
import { RoutesModule } from "@/store/modules/routes";
import i18n from "@/lang"; // Internationalization
import settings from "../settings";
import router from "./index";
import pageList from "@/pages/index";
NProgress.configure({ showSpinner: false });
var whiteList = ["/login", "/auth-redirect"];
var getPageTitle = function (key) {
    var hasKey = i18n.te("route." + key);
    if (hasKey) {
        var pageName = i18n.t("route." + key);
        return pageName + " - " + settings.title;
    }
    return "" + settings.title;
};
var bindLang = function (_a) {
    var zh = _a.zh, en = _a.en;
    var localKey = Object.keys(zh).length > 0 ? Object.keys(zh)[0] : "";
    if (localKey && !i18n.getLocaleMessage('en')[localKey]) {
        i18n.mergeLocaleMessage("en", en);
        i18n.mergeLocaleMessage("zh", zh);
    }
};
router.beforeEach(function (to, _, next) { return __awaiter(void 0, void 0, void 0, function () {
    var loadI18n, err_1;
    return __generator(this, function (_a) {
        switch (_a.label) {
            case 0:
                loadI18n = function (isFirst) {
                    if (isFirst === void 0) { isFirst = false; }
                    // 加载语言
                    import("@/pages" + to.path + "/local.ts").then(function (LOCAL) {
                        bindLang(LOCAL.default);
                        isFirst ? next(__assign(__assign({}, to), { replace: true })) : next();
                    }).catch(function () {
                        console.warn('找不到 多语言文件');
                        isFirst ? next(__assign(__assign({}, to), { replace: true })) : next();
                    });
                };
                NProgress.start();
                if (!UserModule.token) return [3 /*break*/, 7];
                if (!(UserModule.roles.length === 0)) return [3 /*break*/, 5];
                _a.label = 1;
            case 1:
                _a.trys.push([1, 3, , 4]);
                return [4 /*yield*/, UserModule.GetUserInfo()];
            case 2:
                _a.sent();
                RoutesModule.PageList(pageList);
                RoutesModule.GenerateRoutes(UserModule.menus);
                router.addRoutes(RoutesModule.dynamicRoutes);
                loadI18n(UserModule.roles.length > 0);
                return [3 /*break*/, 4];
            case 3:
                err_1 = _a.sent();
                Message.error(err_1 || "Has Error");
                location.href = "/login.html";
                // next(`/login?redirect=${to.path}`);
                NProgress.done();
                return [3 /*break*/, 4];
            case 4: return [3 /*break*/, 6];
            case 5:
                if (to.matched.length !== 0) {
                    loadI18n();
                }
                else {
                    next("/404");
                }
                _a.label = 6;
            case 6: return [3 /*break*/, 8];
            case 7:
                // Has no token
                if (whiteList.indexOf(to.path) !== -1) {
                    loadI18n();
                }
                else {
                    // next(`/login?redirect=${to.path}`);
                    location.href = "/login.html?redirect=" + to.path;
                    NProgress.done();
                }
                _a.label = 8;
            case 8: return [2 /*return*/];
        }
    });
}); });
router.afterEach(function (to) {
    // Finish progress bar
    NProgress.done();
    // set page title
    document.title = getPageTitle(to.meta.title);
});
//# sourceMappingURL=permission.js.map
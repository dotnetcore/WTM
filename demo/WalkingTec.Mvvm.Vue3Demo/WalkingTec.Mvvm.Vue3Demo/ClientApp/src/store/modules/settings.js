import { __assign, __decorate, __extends, __metadata } from "tslib";
import { VuexModule, Module, Mutation, Action, getModule } from "vuex-module-decorators";
import store from "@/store/modules/index";
import { style as variables } from "@/config/index";
import { setSettings, getSettings } from "@/util/cookie";
import defaultSettings from "@/settings";
var Settings = /** @class */ (function (_super) {
    __extends(Settings, _super);
    function Settings() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.title = defaultSettings.title;
        _this.theme = variables.theme;
        _this.fixedHeader = defaultSettings.fixedHeader;
        _this.showSettings = defaultSettings.showSettings;
        _this.showTagsView = defaultSettings.showTagsView;
        _this.showSidebarLogo = defaultSettings.showSidebarLogo;
        _this.sidebarTextTheme = defaultSettings.sidebarTextTheme;
        _this.isDialog = defaultSettings.isDialog;
        _this.menuBackgroundImg = defaultSettings.menuBackgroundImg;
        return _this;
    }
    Settings.prototype.CHANGE_SETTING = function (payload) {
        var _a, _b;
        var key = payload.key, value = payload.value;
        if (Object.prototype.hasOwnProperty.call(this, key)) {
            this[key] = value;
            var obj = getSettings();
            console.log('{...obj, [key]: value}', __assign(__assign({}, obj), (_a = {}, _a[key] = value, _a)));
            setSettings(__assign(__assign({}, obj), (_b = {}, _b[key] = value, _b)));
        }
    };
    Settings.prototype.ChangeSetting = function (payload) {
        this.CHANGE_SETTING(payload);
    };
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], Settings.prototype, "CHANGE_SETTING", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], Settings.prototype, "ChangeSetting", null);
    Settings = __decorate([
        Module({ dynamic: true, store: store, name: "settings" })
    ], Settings);
    return Settings;
}(VuexModule));
export var SettingsModule = getModule(Settings);
//# sourceMappingURL=settings.js.map
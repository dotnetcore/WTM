import { __decorate, __extends, __metadata } from "tslib";
import { VuexModule, Module, Mutation, Action, getModule } from "vuex-module-decorators";
import { getSidebarStatus, getSize, setSidebarStatus, setLanguage, setSize } from "@/util/cookie";
import { getLocale } from "@/lang";
import store from "@/store/modules/index";
export var DeviceType;
(function (DeviceType) {
    DeviceType[DeviceType["Mobile"] = 0] = "Mobile";
    DeviceType[DeviceType["Desktop"] = 1] = "Desktop";
})(DeviceType || (DeviceType = {}));
var App = /** @class */ (function (_super) {
    __extends(App, _super);
    function App() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.sidebar = {
            opened: getSidebarStatus() !== "closed",
            withoutAnimation: false
        };
        _this.device = DeviceType.Desktop;
        _this.language = getLocale();
        _this.size = getSize() || "small";
        return _this;
    }
    App.prototype.TOGGLE_SIDEBAR = function (withoutAnimation) {
        this.sidebar.opened = !this.sidebar.opened;
        this.sidebar.withoutAnimation = withoutAnimation;
        if (this.sidebar.opened) {
            setSidebarStatus("opened");
        }
        else {
            setSidebarStatus("closed");
        }
    };
    App.prototype.CLOSE_SIDEBAR = function (withoutAnimation) {
        this.sidebar.opened = false;
        this.sidebar.withoutAnimation = withoutAnimation;
        setSidebarStatus("closed");
    };
    App.prototype.TOGGLE_DEVICE = function (device) {
        this.device = device;
    };
    App.prototype.SET_LANGUAGE = function (language) {
        this.language = language;
        setLanguage(this.language);
    };
    App.prototype.SET_SIZE = function (size) {
        this.size = size;
        setSize(this.size);
    };
    App.prototype.ToggleSideBar = function (withoutAnimation) {
        this.TOGGLE_SIDEBAR(withoutAnimation);
    };
    App.prototype.CloseSideBar = function (withoutAnimation) {
        this.CLOSE_SIDEBAR(withoutAnimation);
    };
    App.prototype.ToggleDevice = function (device) {
        this.TOGGLE_DEVICE(device);
    };
    App.prototype.SetLanguage = function (language) {
        this.SET_LANGUAGE(language);
    };
    App.prototype.SetSize = function (size) {
        this.SET_SIZE(size);
    };
    App.prototype.ImportPage = function (url) {
        var languagePage = this.language === 'en' ? '.en' : '';
        return import("" + url + languagePage + ".vue").catch(function () {
            return import(url + ".vue");
        });
    };
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Boolean]),
        __metadata("design:returntype", void 0)
    ], App.prototype, "TOGGLE_SIDEBAR", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Boolean]),
        __metadata("design:returntype", void 0)
    ], App.prototype, "CLOSE_SIDEBAR", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Number]),
        __metadata("design:returntype", void 0)
    ], App.prototype, "TOGGLE_DEVICE", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [String]),
        __metadata("design:returntype", void 0)
    ], App.prototype, "SET_LANGUAGE", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [String]),
        __metadata("design:returntype", void 0)
    ], App.prototype, "SET_SIZE", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Boolean]),
        __metadata("design:returntype", void 0)
    ], App.prototype, "ToggleSideBar", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Boolean]),
        __metadata("design:returntype", void 0)
    ], App.prototype, "CloseSideBar", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Number]),
        __metadata("design:returntype", void 0)
    ], App.prototype, "ToggleDevice", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [String]),
        __metadata("design:returntype", void 0)
    ], App.prototype, "SetLanguage", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [String]),
        __metadata("design:returntype", void 0)
    ], App.prototype, "SetSize", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [String]),
        __metadata("design:returntype", Object)
    ], App.prototype, "ImportPage", null);
    App = __decorate([
        Module({ dynamic: true, store: store, name: "app" })
    ], App);
    return App;
}(VuexModule));
export var AppModule = getModule(App);
//# sourceMappingURL=app.js.map
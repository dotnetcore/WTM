import { __decorate, __extends, __metadata } from "tslib";
import { Component, Vue, Watch } from 'vue-property-decorator';
import { AppModule, DeviceType } from '@/store/modules/app';
var WIDTH = 992; // refer to Bootstrap's responsive design
var default_1 = /** @class */ (function (_super) {
    __extends(default_1, _super);
    function default_1() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    Object.defineProperty(default_1.prototype, "device", {
        get: function () {
            return AppModule.device;
        },
        enumerable: false,
        configurable: true
    });
    Object.defineProperty(default_1.prototype, "sidebar", {
        get: function () {
            return AppModule.sidebar;
        },
        enumerable: false,
        configurable: true
    });
    default_1.prototype.onRouteChange = function () {
        if (this.device === DeviceType.Mobile && this.sidebar.opened) {
            AppModule.CloseSideBar(false);
        }
    };
    default_1.prototype.beforeMount = function () {
        window.addEventListener('resize', this.resizeHandler);
    };
    default_1.prototype.mounted = function () {
        var isMobile = this.isMobile();
        if (isMobile) {
            AppModule.ToggleDevice(DeviceType.Mobile);
            AppModule.CloseSideBar(true);
        }
    };
    default_1.prototype.beforeDestroy = function () {
        window.removeEventListener('resize', this.resizeHandler);
    };
    default_1.prototype.isMobile = function () {
        var rect = document.body.getBoundingClientRect();
        return rect.width - 1 < WIDTH;
    };
    default_1.prototype.resizeHandler = function () {
        if (!document.hidden) {
            var isMobile = this.isMobile();
            AppModule.ToggleDevice(isMobile ? DeviceType.Mobile : DeviceType.Desktop);
            if (isMobile) {
                AppModule.CloseSideBar(true);
            }
        }
    };
    __decorate([
        Watch('$route'),
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", void 0)
    ], default_1.prototype, "onRouteChange", null);
    default_1 = __decorate([
        Component({
            name: 'ResizeMixin'
        })
    ], default_1);
    return default_1;
}(Vue));
export default default_1;
//# sourceMappingURL=resize.js.map
import { __decorate, __extends, __metadata } from "tslib";
import { VuexModule, Module, Mutation, Action, getModule } from "vuex-module-decorators";
import store from "@/store/modules/index";
var ErrorLog = /** @class */ (function (_super) {
    __extends(ErrorLog, _super);
    function ErrorLog() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.logs = [];
        return _this;
    }
    ErrorLog.prototype.ADD_ERROR_LOG = function (log) {
        this.logs.push(log);
    };
    ErrorLog.prototype.CLEAR_ERROR_LOG = function () {
        this.logs.splice(0);
    };
    ErrorLog.prototype.AddErrorLog = function (log) {
        this.ADD_ERROR_LOG(log);
    };
    ErrorLog.prototype.ClearErrorLog = function () {
        this.CLEAR_ERROR_LOG();
    };
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], ErrorLog.prototype, "ADD_ERROR_LOG", null);
    __decorate([
        Mutation,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", void 0)
    ], ErrorLog.prototype, "CLEAR_ERROR_LOG", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", [Object]),
        __metadata("design:returntype", void 0)
    ], ErrorLog.prototype, "AddErrorLog", null);
    __decorate([
        Action,
        __metadata("design:type", Function),
        __metadata("design:paramtypes", []),
        __metadata("design:returntype", void 0)
    ], ErrorLog.prototype, "ClearErrorLog", null);
    ErrorLog = __decorate([
        Module({ dynamic: true, store: store, name: "errorLog" })
    ], ErrorLog);
    return ErrorLog;
}(VuexModule));
export var ErrorLogModule = getModule(ErrorLog);
//# sourceMappingURL=error-log.js.map
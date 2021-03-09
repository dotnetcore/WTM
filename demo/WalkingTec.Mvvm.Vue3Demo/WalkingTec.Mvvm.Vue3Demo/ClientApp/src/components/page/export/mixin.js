import { __decorate, __extends, __metadata } from "tslib";
import { Component, Vue } from "vue-property-decorator";
import { Action, Mutation, State } from "vuex-class";
var ExportExcel = /** @class */ (function (_super) {
    __extends(ExportExcel, _super);
    function ExportExcel() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    __decorate([
        State("session"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "session", void 0);
    __decorate([
        State("progress"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "progress", void 0);
    __decorate([
        State("isFinish"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "isFinish", void 0);
    __decorate([
        State("exceed"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "exceed", void 0);
    __decorate([
        State("exceedMsg"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "exceedMsg", void 0);
    __decorate([
        State("position"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "position", void 0);
    __decorate([
        State("downloadUrl"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "downloadUrl", void 0);
    __decorate([
        Mutation("setSession"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "setSession", void 0);
    __decorate([
        Mutation("setProgress"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "setProgress", void 0);
    __decorate([
        Mutation("setIsFinish"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "setIsFinish", void 0);
    __decorate([
        Mutation("setPosition"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "setPosition", void 0);
    __decorate([
        Mutation("setExceed"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "setExceed", void 0);
    __decorate([
        Mutation("setExceedMsg"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "setExceedMsg", void 0);
    __decorate([
        Action("getExportInfo"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "getExportInfo", void 0);
    __decorate([
        Action("getProgress"),
        __metadata("design:type", Object)
    ], ExportExcel.prototype, "getProgress", void 0);
    ExportExcel = __decorate([
        Component
    ], ExportExcel);
    return ExportExcel;
}(Vue));
export default ExportExcel;
//# sourceMappingURL=mixin.js.map
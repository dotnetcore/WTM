import { __assign, __decorate, __extends, __metadata } from "tslib";
import { Component, Vue } from "vue-property-decorator";
import { listToString } from "@/util/string";
import { createBlob } from "@/util/files";
import UploadBox from "@/components/page/upload/index.vue";
import { Action, State } from "vuex-class";
/**
 * 首页中的按钮部分，添加/修改/删除/导出/导出
 */
function mixinFunc(ASSEMBLIES) {
    if (ASSEMBLIES === void 0) { ASSEMBLIES = []; }
    var actionMixins = /** @class */ (function (_super) {
        __extends(actionMixins, _super);
        function actionMixins() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            // 展示的动作按钮
            _this.assembly = ASSEMBLIES;
            // 表单弹出框内容
            _this.dialogIsShow = false;
            // 打开选中数据
            _this.dialogData = {};
            // 打开详情状态（增删改查）
            _this.dialogStatus = "";
            // 导入
            _this.uploadIsShow = false;
            return _this;
        }
        Object.defineProperty(actionMixins.prototype, "actionEvent", {
            /**
             * 事件方法list
             */
            get: function () {
                return {
                    onAdd: this.onAdd,
                    onEdit: this.onEdit,
                    onDetail: this.onDetail,
                    onDelete: this.onDelete,
                    onBatchDelete: this.onBatchDelete,
                    onImported: this.onImported,
                    onExportAll: this.onExportAll,
                    onExport: this.onExport
                };
            },
            enumerable: false,
            configurable: true
        });
        /**
         * 打开详情弹框（默认框）
         * @param status
         * @param data
         */
        actionMixins.prototype.openDialog = function (status, data) {
            if (data === void 0) { data = {}; }
            this.dialogIsShow = true;
            this.dialogStatus = status;
            this.dialogData = data;
        };
        /**
         * 查询接口 ★★★★★
         * @param params
         */
        actionMixins.prototype.privateRequest = function (params) {
            return this.search(params);
        };
        /**
         * 添加
         */
        actionMixins.prototype.onAdd = function () {
            this.openDialog(this.$actionType.add);
        };
        /**
         * 修改
         * @param data
         */
        actionMixins.prototype.onEdit = function (data) {
            this.openDialog(this.$actionType.edit, { ID: data.ID });
        };
        /**
         * 详情
         * @param data
         */
        actionMixins.prototype.onDetail = function (data) {
            this.openDialog(this.$actionType.detail, { ID: data.ID });
        };
        /**
         * 单个删除
         * @param params
         */
        actionMixins.prototype.onDelete = function (params) {
            var _this = this;
            this.onConfirm().then(function () {
                var parameters = [params.ID];
                _this.batchDelete(parameters).then(function (res) {
                    _this["$notify"]({
                        title: _this.$t("form.successfullyDeleted"),
                        type: "success"
                    });
                    _this["onHoldSearch"]();
                });
            });
        };
        /**
         * 多个删除
         */
        actionMixins.prototype.onBatchDelete = function () {
            var _this = this;
            this.onConfirm().then(function () {
                var parameters = listToString(_this["selectData"], "ID");
                console.log('this["selectData"]', _this["selectData"], parameters);
                _this.batchDelete(parameters)
                    .then(function (res) {
                    _this["$notify"]({
                        title: _this.$t("form.successfullyDeleted"),
                        type: "success"
                    });
                    _this["onHoldSearch"]();
                })
                    .catch(function (err) {
                    _this["$notify"]({
                        title: _this.$t("form.failedToDelete"),
                        type: "error"
                    });
                });
            });
        };
        /**
         * 导出全部
         */
        actionMixins.prototype.onExportAll = function () {
            var _this = this;
            var parameters = __assign(__assign({}, this["searchFormClone"]), { Page: this["pageDate"].currentPage, Limit: this["pageDate"].pageSize });
            this.exportExcel(parameters).then(function (res) {
                createBlob(res);
                _this["$notify"]({
                    title: _this.$t("form.ExportSucceeded"),
                    type: "success"
                });
            });
        };
        /**
         * 导出单个
         */
        actionMixins.prototype.onExport = function () {
            var _this = this;
            var parameters = listToString(this["selectData"], "ID");
            this.exportExcelByIds(parameters).then(function (res) {
                createBlob(res);
                _this["$notify"]({
                    title: _this.$t("form.ExportSucceeded"),
                    type: "success"
                });
            });
        };
        /**
         * open importbox
         */
        actionMixins.prototype.onImported = function () {
            this.uploadIsShow = true;
        };
        /**
         * 下载
         */
        actionMixins.prototype.onDownload = function () {
            this.getExcelTemplate().then(function (res) { return createBlob(res); });
        };
        /**
         * 导入
         * @param fileData
         */
        actionMixins.prototype.onImport = function (fileData) {
            var _this = this;
            var parameters = {
                UploadFileId: fileData.Id
            };
            this.imported(parameters).then(function (res) {
                _this["$notify"]({
                    title: _this.$t("form.ImportSucceeded"),
                    type: "success"
                });
                _this["onHoldSearch"]();
            });
        };
        /**
         * 删除确认
         * @param title
         */
        actionMixins.prototype.onConfirm = function (title) {
            if (!title) {
                title = this.$t("form.confirmDeletion");
            }
            return this["$confirm"](title, this.$t("form.prompt"), {
                confirmButtonText: this.$t("buttom.delete"),
                cancelButtonText: this.$t("buttom.cancel"),
                type: "warning"
            });
        };
        __decorate([
            Action("search"),
            __metadata("design:type", Object)
        ], actionMixins.prototype, "search", void 0);
        __decorate([
            Action("batchDelete"),
            __metadata("design:type", Object)
        ], actionMixins.prototype, "batchDelete", void 0);
        __decorate([
            Action("deleted"),
            __metadata("design:type", Object)
        ], actionMixins.prototype, "deleted", void 0);
        __decorate([
            Action("exportExcel"),
            __metadata("design:type", Object)
        ], actionMixins.prototype, "exportExcel", void 0);
        __decorate([
            Action("exportExcelByIds"),
            __metadata("design:type", Object)
        ], actionMixins.prototype, "exportExcelByIds", void 0);
        __decorate([
            Action("detail"),
            __metadata("design:type", Object)
        ], actionMixins.prototype, "detail", void 0);
        __decorate([
            Action("imported"),
            __metadata("design:type", Object)
        ], actionMixins.prototype, "imported", void 0);
        __decorate([
            Action("getExcelTemplate"),
            __metadata("design:type", Object)
        ], actionMixins.prototype, "getExcelTemplate", void 0);
        __decorate([
            State,
            __metadata("design:type", Object)
        ], actionMixins.prototype, "actionList", void 0);
        actionMixins = __decorate([
            Component({
                components: {
                    UploadBox: UploadBox
                }
            })
        ], actionMixins);
        return actionMixins;
    }(Vue));
    return actionMixins;
}
export default mixinFunc;
//# sourceMappingURL=action-mixin.js.map
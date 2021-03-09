import { __decorate, __extends, __metadata } from "tslib";
import { Component, Vue, Prop } from "vue-property-decorator";
import { Action } from "vuex-class";
/**
 * 弹出框（详情/编辑/创建）
 * dialogData：被编辑数据
 * status：弹出框的状态
 * actionType: 弹出框的状态-枚举
 * 注：
 *     目前与CreateForm组件高度依赖，做一需要配合CreateForm组件
 */
function mixinFunc(defaultRefName) {
    if (defaultRefName === void 0) { defaultRefName = "el_form_name"; }
    var formMixins = /** @class */ (function (_super) {
        __extends(formMixins, _super);
        function formMixins() {
            var _this = _super !== null && _super.apply(this, arguments) || this;
            /**
             * 补充表单数据,onAdd|onEdit会把当前数据合并到提交表单（formData）中
             * 例如：
             *    创建表单（CreateForm）时，需要用到自定义组件(type: wtmSlot),
             *    可以将自定义组件的数据 放到mergeFormData中操作，查询/编辑/新增 动作都会填充数据项
             */
            _this.mergeFormData = {};
            /**
             * 表单ref name
             */
            _this.refName = defaultRefName;
            /**
             * 异步验证, 失败组件集合(form-item类型是error需要)
             */
            _this.asynValidateEl = [];
            return _this;
        }
        Object.defineProperty(formMixins.prototype, "formEvent", {
            /**
             * wtm-dialog-box所需方法
             */
            get: function () {
                return {
                    close: this.onClose,
                    opened: this.onOpen,
                    onSubmit: this.onSubmit
                };
            },
            enumerable: false,
            configurable: true
        });
        /**
         * 返回表单组件, this.$refs，get监听不到，改为方法
         */
        formMixins.prototype.FormComp = function () {
            return _.get(this.$refs, this.refName);
        };
        /**
         * 关闭
         */
        formMixins.prototype.onClose = function () {
            this.$emit("update:isShow", false);
            this.onReset();
        };
        /**
         * 重置&清除验证
         */
        formMixins.prototype.onReset = function () {
            var comp = this.FormComp();
            if (comp) {
                this.asynValidateEl.forEach(function (key) {
                    return comp.getFormItem(key).clearValidate();
                });
                comp.resetFields();
                comp.setFormDataItem('Entity.ID', '');
            }
        };
        /**
         * 打开详情
         */
        formMixins.prototype.onOpen = function () {
            var _this = this;
            if (!this.dialogData) {
                console.warn("dialogData 没有id数据", this.dialogData);
            }
            var params = this.beforeOpen(this.dialogData) || this.dialogData;
            if (this["status"] !== this["$actionType"].add) {
                this["detail"](params).then(function (res) {
                    _this.setFormData(res);
                    _this.afterOpen(res);
                });
            }
            else {
                this.onReset();
                this.afterOpen();
            }
        };
        /**
         * 查询详情-绑定数据 之前
         */
        formMixins.prototype.beforeOpen = function (data) {
            console.log("data:", data);
            return data;
        };
        /**
         * 查询详情-绑定数据 之后
         */
        formMixins.prototype.afterOpen = function (data) {
            console.log("data:", data);
        };
        /**
         * 提交
         */
        formMixins.prototype.onSubmit = function () {
            var _this = this;
            this.FormComp().validate(function (valid, data) {
                if (valid) {
                    if (_this.status === _this["$actionType"].add) {
                        _this.onAdd(data);
                    }
                    else if (_this.status === _this["$actionType"].edit) {
                        _this.onEdit(data);
                    }
                }
            });
        };
        /**
         * 添加
         */
        formMixins.prototype.onAdd = function (data) {
            var _this = this;
            if (data === void 0) { data = null; }
            var formData = this.getFormData(data);
            if (!formData.Entity["ID"]) {
                delete formData.Entity["ID"];
            }
            formData = this.beforeRequest(formData) || formData;
            return this.add(formData)
                .then(function (res) {
                _this["$notify"]({
                    title: "添加成功",
                    type: "success"
                });
                _this.onClose();
                _this.$emit("onSearch");
            })
                .catch(function (error) {
                _this.showResponseValidate(error.response.data.Form);
            });
        };
        /**
         * 编辑
         */
        formMixins.prototype.onEdit = function (data) {
            var _this = this;
            if (data === void 0) { data = null; }
            var formData = this.getFormData(data);
            formData = this.beforeRequest(formData) || formData;
            return this.edit(formData)
                .then(function (res) {
                _this["$notify"]({
                    title: _this.$t("form.SuccessfullyModified"),
                    type: "success"
                });
                _this.onClose();
                _this.$emit("onSearch");
            })
                .catch(function (error) {
                _this.showResponseValidate(error.response.data.Form);
            });
        };
        /**
         * submit请求 之前
         */
        formMixins.prototype.beforeRequest = function (formData) {
            console.log("beforeRequest:", formData);
            return formData;
        };
        /**
         * get merge formdata
         */
        formMixins.prototype.getFormData = function (data) {
            if (data === void 0) { data = null; }
            var formData = data || this.FormComp().getFormData();
            // 处理 array
            var customizer = function (objValue, srcValue) {
                if (_.isArray(objValue)) {
                    return srcValue;
                }
            };
            formData = _.mergeWith(formData, this.mergeFormData, customizer);
            return formData;
        };
        /**
         * set
         */
        formMixins.prototype.setFormData = function (data) {
            var _this = this;
            // 填充表单数据
            var originData = this.FormComp().setFormData(data);
            // 填充补充表单数据
            _.mapKeys(originData, function (value, key) {
                if (_.get(_this.mergeFormData, key) !== undefined) {
                    _.set(_this.mergeFormData, key, _.cloneDeep(value));
                }
            });
        };
        /**
         * 展示接口 验证错误提示
         */
        formMixins.prototype.showResponseValidate = function (resForms) {
            var _this = this;
            _.mapKeys(resForms, function (value, key) {
                var formItem = _this.FormComp().getFormItem(key);
                if (formItem) {
                    formItem.showError(value);
                    _this.asynValidateEl.push(key);
                }
            });
        };
        __decorate([
            Action("add"),
            __metadata("design:type", Object)
        ], formMixins.prototype, "add", void 0);
        __decorate([
            Action("edit"),
            __metadata("design:type", Object)
        ], formMixins.prototype, "edit", void 0);
        __decorate([
            Action("detail"),
            __metadata("design:type", Object)
        ], formMixins.prototype, "detail", void 0);
        __decorate([
            Prop({ type: Object, default: function () { } }),
            __metadata("design:type", Object)
        ], formMixins.prototype, "dialogData", void 0);
        __decorate([
            Prop({ type: String, default: "" }),
            __metadata("design:type", Object)
        ], formMixins.prototype, "status", void 0);
        __decorate([
            Prop({ type: Boolean, default: false }),
            __metadata("design:type", Object)
        ], formMixins.prototype, "isShow", void 0);
        formMixins = __decorate([
            Component
        ], formMixins);
        return formMixins;
    }(Vue));
    return formMixins;
}
export default mixinFunc;
//# sourceMappingURL=form-mixin.js.map
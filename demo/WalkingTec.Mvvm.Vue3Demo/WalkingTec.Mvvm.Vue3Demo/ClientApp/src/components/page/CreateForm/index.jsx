import { __assign, __decorate, __extends, __metadata } from "tslib";
import { Component, Prop, Vue } from "vue-property-decorator";
import Utils from "./utils";
import WtmUploadImg from "@/components/page/UploadImg.vue";
// 组件集合
var componentObj = new Utils();
/**
 * 创建form
 *
 * 注：
 *    formData虽然是prop参数，但多层级，不符合vue规范，但可以实现父级数据同步；
 *
 */
var CreateForm = /** @class */ (function (_super) {
    __extends(CreateForm, _super);
    function CreateForm() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        /**
         * key替换'.'之后的数据
         */
        _this.formData = {};
        /**
         * 事件集合 @Prop({ type: Object, default: () => {} }) events;
         */
        _this.elFormRefKey = "ref_name";
        return _this;
    }
    Object.defineProperty(CreateForm.prototype, "elForm", {
        /**
         * 透传el-form组件
         */
        get: function () {
            var refForm = _.get(this.$refs, this.elFormRefKey);
            return refForm;
        },
        enumerable: false,
        configurable: true
    });
    /**
     * 透传el-form，validate事件
     * 重定义callback，验证通过，data参数 改为表单数据
     */
    CreateForm.prototype.validate = function (callback) {
        var _this = this;
        var refForm = _.get(this.$refs, this.elFormRefKey);
        refForm.validate(function (valid, data) {
            if (valid) {
                callback(true, _this.getFormData());
            }
            else {
                callback(valid, data);
            }
        });
    };
    /**
     * 清空el-form验证
     */
    CreateForm.prototype.resetFields = function () {
        var refForm = _.get(this.$refs, this.elFormRefKey);
        return refForm.resetFields();
    };
    /**
     * 'Entity.ID' => Entity: { ID }
     */
    CreateForm.prototype.getFormData = function () {
        var deep = _.cloneDeep(this.sourceFormData || this.formData);
        var formOptions = this.options.formItem;
        for (var key in formOptions) {
            var option = formOptions[key];
            if (_.isBoolean(option['isFileDataById']) && !option['isFileDataById']) {
                var value = _.get(deep, key) || [];
                _.set(deep, key, value.map(function (item) { return item.response.Id || item.Id; }));
            }
        }
        return deep;
    };
    /**
     *  Entity: { ID } => 'Entity.ID'
     */
    CreateForm.prototype.setFormData = function (data) {
        var _this = this;
        var pointData = {};
        _.mapKeys(this.options.formItem, function (value, key) {
            _this.setFormDataItem(key, _.get(data, key));
            pointData[key] = _.get(data, key);
        });
        return pointData;
    };
    /**
     * 赋值formData
     * @param path 字段对应路径 a: { b: { c: 1}} , path = a.b.c
     * @param value
     */
    CreateForm.prototype.setFormDataItem = function (path, value) {
        _.set(this.sourceFormData || this.formData, path, value);
    };
    /**
     * 返回wtmformItem
     * @param key
     */
    CreateForm.prototype.getFormItem = function (key) {
        return this.$refs[key];
    };
    /**
     * 多语言
     * @param option
     */
    CreateForm.prototype.getLanguageByKey = function (_a) {
        var label = _a.label, key = _a.key;
        return this.$getLanguageByKey({
            languageKey: this.languageKey,
            label: label,
            key: key
        });
    };
    CreateForm.prototype.createFormData = function () {
        var newFormData = {};
        var formItem = this.options.formItem;
        _.mapKeys(formItem, function (item, key) {
            var value = "";
            if (_.isNil(item.defaultValue)) {
                value = ["switch"].includes(item.type) ? true : "";
            }
            else {
                value = item.defaultValue;
            }
            _.setWith(newFormData, key, value);
        });
        return newFormData;
    };
    CreateForm.prototype.created = function () {
        this.formData = this.createFormData();
    };
    CreateForm.prototype.render = function (h) {
        var _this = this;
        var components = _.keys(this.options.formItem).map(function (key) {
            var item = _this.options.formItem[key];
            if (_.isFunction(item.isHidden)) {
                if (item.isHidden(_this.getFormData(), _this.status)) {
                    return;
                }
            }
            if ((_.isBoolean(item.isHidden) && item.isHidden) || !item.type) {
                return;
            }
            var itemComp = componentObj[item.type];
            var option = __assign(__assign({}, item), { key: key });
            var contentComp = itemComp ? itemComp.call(_this, h, option) : null;
            return componentObj.wtmFormItem.call(_this, h, option, contentComp);
        });
        var props = __assign(__assign({}, this.options.formProps), { disabled: this.status === "detail", model: this.sourceFormData || this.formData });
        var slots = this.$scopedSlots["default"];
        return (<el-form ref={this.elFormRefKey} {...{ props: props }}>
        <el-row class={this.elRowClass}>
          {components}
          {slots && slots({})}
        </el-row>
      </el-form>);
    };
    __decorate([
        Prop({ type: String, default: "add" }),
        __metadata("design:type", Object)
    ], CreateForm.prototype, "status", void 0);
    __decorate([
        Prop(),
        __metadata("design:type", Object)
    ], CreateForm.prototype, "options", void 0);
    __decorate([
        Prop({ type: String, default: "" }),
        __metadata("design:type", Object)
    ], CreateForm.prototype, "elRowClass", void 0);
    __decorate([
        Prop(),
        __metadata("design:type", Object)
    ], CreateForm.prototype, "sourceFormData", void 0);
    __decorate([
        Prop({ type: Boolean, default: false }),
        __metadata("design:type", Object)
    ], CreateForm.prototype, "elDisabled", void 0);
    __decorate([
        Prop({ type: String }),
        __metadata("design:type", Object)
    ], CreateForm.prototype, "languageKey", void 0);
    CreateForm = __decorate([
        Component({
            components: {
                "wtm-upload-img": WtmUploadImg,
            },
        })
    ], CreateForm);
    return CreateForm;
}(Vue));
export default CreateForm;
// private createFormData() {
// let formData = {};
// const formItem = this.options.formItem;
// _.mapKeys(formItem, (valule, key) => {
//   const newKey = this.KeyByString(key);
//   if (_.isNil(valule.defaultValue)) {
//     formData[newKey] = ["switch"].includes(valule.type) ? true : "";
//   } else {
//     formData[newKey] = valule.defaultValue;
//   }
// });
// return formData;
// }
// /**
//  * e.key => e_partition_key
//  */
// private KeyByPoint(key): string {
//   return key.replace(/_partition_/g, ".");
// }
// /**
//  * e_partition_key => e.key
//  */
// private KeyByString(key): string {
//   return key.replace(/(\.)/g, "_partition_");
// }
//# sourceMappingURL=index.jsx.map
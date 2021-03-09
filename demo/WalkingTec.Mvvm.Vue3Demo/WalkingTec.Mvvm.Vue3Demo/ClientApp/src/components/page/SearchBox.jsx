import { __decorate, __extends, __metadata } from "tslib";
import { Component, Prop, Vue, Provide } from "vue-property-decorator";
/**
 *
 */
var CreateForm = /** @class */ (function (_super) {
    __extends(CreateForm, _super);
    function CreateForm() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.componentName = "wtmSearch";
        _this.refName = "searchRefName";
        /**
         * 异步验证, 失败组件集合(form-item类型是error需要)
         */
        _this.asynValidateEl = [];
        return _this;
    }
    /**
     * 返回表单组件, this.$refs，get监听不到，改为方法
     */
    CreateForm.prototype.FormComp = function () {
        return _.get(this.$refs, this.refName);
    };
    /**
     * 表单数据
     */
    CreateForm.prototype.getFormData = function () {
        return this.FormComp().getFormData();
    };
    /**
     * 返回wtmformItem
     * @param key
     */
    CreateForm.prototype.getFormItem = function (key) {
        return this.FormComp().getFormItem(key);
    };
    /**
     * 展示接口 验证错误提示
     */
    CreateForm.prototype.showResponseValidate = function (resForms) {
        var _this = this;
        _.mapKeys(resForms, function (value, key) {
            var formItem = _this.getFormItem(key);
            if (formItem) {
                formItem.showError(value);
                _this.asynValidateEl.push(key);
            }
        });
    };
    CreateForm.prototype.onSearch = function () {
        var _this = this;
        this.asynValidateEl.forEach(function (key) {
            return _this.getFormItem(key).clearValidate();
        });
        if (this.events && this.events["onSearch"]) {
            this.events["onSearch"]();
        }
        else {
            this.$emit("onSearch");
        }
    };
    CreateForm.prototype.toggleCollapse = function () {
        this.$emit("update:isActive", !this.isActive);
    };
    CreateForm.prototype.onReset = function () {
        this.FormComp().resetFields();
        this.onSearch();
    };
    Object.defineProperty(CreateForm.prototype, "slotList", {
        get: function () {
            var arr = [];
            _.mapValues(this.formOptions.formItem, function (item) {
                if (item.slotKey) {
                    arr.push(item.slotKey);
                }
            });
            return arr;
        },
        enumerable: false,
        configurable: true
    });
    CreateForm.prototype.render = function (h) {
        var _this = this;
        var arr = [];
        _.mapValues(this.formOptions.formItem, function (item) {
            if (item.slotKey) {
                var fn = _this.$scopedSlots[item.slotKey] || (function () { });
                arr.push({ key: item.slotKey, value: fn({}) });
            }
        });
        return (<el-card class="search-box" shadow="never">
        <wtm-create-form ref={this.refName} options={this.formOptions} sourceFormData={this.sourceFormData} elRowClass="flex-container">
          {arr.map(function (item) {
            return <span slot={item.key}>{item.value}</span>;
        })}
          <wtm-form-item class="search-but-box">
            <el-button-group class="button-group">
              <el-button type="primary" class="btn-search" icon="el-icon-search" disabled={this.disabledInput} on-click={this.onSearch}>
                {this.$t("buttom.search")}
              </el-button>
              {this.needCollapse ? (<el-button type="primary" on-click={this.toggleCollapse} style="padding-left: 7px; padding-right: 7px;">
                  <i class={{
            "is-active": this.isActive,
            fa: true,
            "arrow-down": true,
            "el-icon-arrow-down": true
        }}/>
                </el-button>) : ("")}
            </el-button-group>
            {this.needResetBtn ? (<el-button style="position: relative;margin-left: 10px;" plain type="primary" icon="el-icon-refresh" on-click={this.onReset}>
                {this.$t("buttom.reset")}
              </el-button>) : ("")}
          </wtm-form-item>
        </wtm-create-form>
      </el-card>);
    };
    __decorate([
        Provide(),
        __metadata("design:type", Object)
    ], CreateForm.prototype, "componentName", void 0);
    __decorate([
        Prop({ type: Object, default: function () { } }),
        __metadata("design:type", Object)
    ], CreateForm.prototype, "formOptions", void 0);
    __decorate([
        Prop(),
        __metadata("design:type", Object)
    ], CreateForm.prototype, "sourceFormData", void 0);
    __decorate([
        Prop({ type: Boolean, default: false }),
        __metadata("design:type", Boolean)
    ], CreateForm.prototype, "needCollapse", void 0);
    __decorate([
        Prop({ type: Boolean, default: false }),
        __metadata("design:type", Boolean)
    ], CreateForm.prototype, "isActive", void 0);
    __decorate([
        Prop({ default: true }),
        __metadata("design:type", Boolean)
    ], CreateForm.prototype, "needResetBtn", void 0);
    __decorate([
        Prop({ default: false }),
        __metadata("design:type", Boolean)
    ], CreateForm.prototype, "disabledInput", void 0);
    __decorate([
        Prop({ type: Object, default: null }),
        __metadata("design:type", Object)
    ], CreateForm.prototype, "events", void 0);
    CreateForm = __decorate([
        Component({
            name: "wtm-search",
            components: {}
        })
    ], CreateForm);
    return CreateForm;
}(Vue));
export default CreateForm;
//# sourceMappingURL=SearchBox.jsx.map
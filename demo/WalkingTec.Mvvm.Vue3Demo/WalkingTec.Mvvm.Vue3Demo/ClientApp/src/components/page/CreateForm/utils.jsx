import { __assign, __spreadArrays } from "tslib";
/**
 * 需要继承vue的this
 *    当前写法支持v-model，但需要将this指向vue
 *
 * 自定义组件
 *  ['WtmFormItem'] /vue-custom/component/index
 *  ['UploadImg'] /components/page/UploadImg.vue
 *
 * 写法-注：
 * model使用
 *    <el-input v-model={formData[key]} />
 * value使用 (参考：transfer)
 *    const on = {
 *      input(val) {
 *        formData[key] = val;
 *      }
 *    };
 *    <el-input value={formData[key]} ...{on}/>
 */
import { actionApi, fileApi } from "@/service/modules/upload";
var Utils = /** @class */ (function () {
    function Utils() {
        this.wtmFormItem = this.generateWtmFormItemComponent;
        this.input = this.generateInputComponent;
        this.select = this.generateSelectComponent;
        this.button = this.generateButtonComponent;
        this.radio = this.generateRadioComponent;
        this.radioGroup = this.generateRadioGroupComponent;
        this.checkbox = this.generateCheckboxComponent;
        this.checkboxGroup = this.generateCheckboxGroupComponent;
        this.switch = this.generateSwitchComponent;
        this.upload = this.generateUploadComponent;
        this.wtmUploadImg = this.generateWtmUploadImgComponent;
        this.wtmSlot = this.generateWtmSlotComponent;
        this.label = this.generateLabelComponent;
        this.datePicker = this.generateDatePickerComponent;
        this.transfer = this.generateTransferComponent;
    }
    /**
     * formItem 继承vue组件this
     * @param h
     * @param option
     * @param component
     * @param vm
     */
    Utils.prototype.generateWtmFormItemComponent = function (h, option, component, vm) {
        var _t = vm || this;
        var isDetail = _t.status === _t.$actionType.detail;
        var attrs = {
            label: _t.getLanguageByKey(option) + (isDetail ? ":" : ""),
            rules: option.rules,
            prop: option.key ? option.key : "",
            error: option.error,
            "label-width": option["label-width"] || option["labelWidth"],
            span: option.span,
            isShow: option.isShow,
            value: option.value,
            isImg: option.isImg,
        };
        // 展示状态 需要操作的组件
        if (isDetail) {
            var value = _.get(_t.sourceFormData || _t.formData, option.key);
            delete attrs.rules;
            // 图片
            if (["wtmUploadImg", "upload"].includes(option.type) && value) {
                var imgs = _.isArray(value) ? value : [value];
                var imgComponents = imgs.map(function (item) {
                    var url = _.isString(item) && !option.mapKey ? item : item[option.mapKey];
                    return (<el-image style={option.props.imageStyle} src={fileApi + url} preview-src-list={[fileApi + url]} fit={option.props.fit || "contain"}></el-image>);
                });
                if (imgComponents) {
                    return (<wtm-form-item ref={option.key} {...{ attrs: attrs, props: attrs }}>
              {imgComponents}
            </wtm-form-item>);
                }
            }
        }
        return (<wtm-form-item ref={option.key} {...{ attrs: attrs, props: attrs }}>
        {component}
      </wtm-form-item>);
    };
    Utils.prototype.generateInputComponent = function (h, option, vm) {
        var _t = vm || this;
        var style = option.style, props = option.props, slot = option.slot, key = option.key;
        var on = translateEvents(option.events, _t);
        var compData = {
            directives: __spreadArrays((option.directives || []), [vEdit(_t)]),
            on: on,
            props: __assign(__assign({}, displayProp(_t)), props),
            style: style,
            slot: slot,
        };
        var placeholder = "" + _t.$t('form.pleaseEnter') + option.label;
        if (props && props.placeholder) {
            placeholder = props.placeholder;
        }
        var vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
        return (<el-input v-model={vmodelData[key]} {...compData} placeholder={placeholder}>
        {slot}
      </el-input>);
    };
    Utils.prototype.generateSelectComponent = function (h, option, vm) {
        var _t = vm || this;
        var style = option.style, props = option.props, key = option.key, mapKey = option.mapKey;
        var components = [];
        if (option.children && _.isArray(option.children)) {
            components = option.children.map(function (child) {
                var value = child.Value;
                var label = child.Text || child.text;
                var slot = undefined;
                if (_.isString(child.slot)) {
                    slot = (<wtm-render-view hml={child.slot} params={{ Value: value, Text: label }}></wtm-render-view>);
                }
                return (<el-option key={child.Value} {...{ props: __assign({ value: value, label: label }, child.props) }}>
            {slot}
          </el-option>);
            });
        }
        var compData = {
            directives: __spreadArrays((option.directives || []), [vEdit(_t, option.children)]),
            on: translateEvents(option.events, _t),
            props: __assign(__assign(__assign({}, displayProp(_t)), { clearable: true }), props),
            style: style,
        };
        // mapkey && 多选
        if (mapKey && props.multiple) {
            compData.on["input"] = function (val) {
                setMapKeyModel(_t, key, val, mapKey);
            };
            var value = getMapKeyModel(_t, key, mapKey);
            return (<el-select value={value} {...compData}>
          {components}
        </el-select>);
        }
        else {
            var vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
            return (<el-select v-model={vmodelData[key]} {...compData}>
          {components}
        </el-select>);
        }
    };
    Utils.prototype.generateButtonComponent = function (h, option) {
        var style = option.style, props = option.props, slot = option.slot, on = option.events, text = option.text;
        return <el-button {...{ style: style, props: props, slot: slot, on: on }}>{text}</el-button>;
    };
    Utils.prototype.generateRadioComponent = function (h, option, vm) {
        var _t = vm || this;
        var style = option.style, props = option.props, slot = option.slot, key = option.key, text = option.text;
        var on = translateEvents(option.events, _t);
        var compData = {
            directives: __spreadArrays((option.directives || []), [vEdit(_t)]),
            on: on,
            props: __assign(__assign({}, displayProp(_t)), props),
            style: style,
            slot: slot,
        };
        var vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
        return (<el-radio v-model={vmodelData[key]} {...compData}>
        {text}
      </el-radio>);
    };
    Utils.prototype.generateRadioGroupComponent = function (h, option, vm) {
        var _t = vm || this;
        var style = option.style, props = option.props, slot = option.slot, key = option.key;
        var components = [];
        if (option.children) {
            components = option.children.map(function (child) {
                var label = child.Value;
                var text = child.Text || child.text;
                return (<el-radio {...{ props: __assign(__assign({}, child.props), { label: label }) }}>{text}</el-radio>);
            });
        }
        var on = translateEvents(option.events, _t);
        var compData = {
            directives: __spreadArrays((option.directives || []), [vEdit(_t, option.children)]),
            on: on,
            props: __assign(__assign({}, displayProp(_t)), props),
            style: style,
            slot: slot,
        };
        var vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
        return (<el-radio-group v-model={vmodelData[key]} {...compData}>
        {components}
      </el-radio-group>);
    };
    Utils.prototype.generateCheckboxComponent = function (h, option, vm) {
        var _t = vm || this;
        var style = option.style, props = option.props, slot = option.slot, key = option.key, text = option.text;
        var on = translateEvents(option.events, _t);
        var compData = {
            directives: __spreadArrays((option.directives || []), [vEdit(_t)]),
            on: on,
            props: __assign(__assign({}, displayProp(_t)), props),
            style: style,
            slot: slot,
        };
        var vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
        return (<el-checkbox v-model={vmodelData[key]} {...compData}>
        {text}
      </el-checkbox>);
    };
    Utils.prototype.generateCheckboxGroupComponent = function (h, option, vm) {
        var _t = vm || this;
        var style = option.style, props = option.props, slot = option.slot, key = option.key, mapKey = option.mapKey;
        var components = [];
        if (option.children) {
            components = option.children.map(function (child) {
                var label = child.Value;
                var text = child.Text || child.text;
                return (<el-checkbox {...{ props: __assign(__assign({}, child.props), { label: label }) }}>
            {text}
          </el-checkbox>);
            });
        }
        var on = translateEvents(option.events, _t);
        var compData = {
            directives: __spreadArrays((option.directives || []), [vEdit(_t)]),
            on: on,
            props: __assign(__assign({}, displayProp(_t)), props),
            style: style,
            slot: slot,
        };
        if (mapKey) {
            compData.on["input"] = function (val) {
                setMapKeyModel(_t, key, val, mapKey);
            };
            var value = getMapKeyModel(_t, key, mapKey);
            return (<el-checkbox-group value={value} {...compData}>
          {components}
        </el-checkbox-group>);
        }
        else {
            var vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
            return (<el-checkbox-group v-model={vmodelData[key]} {...compData}>
          {components}
        </el-checkbox-group>);
        }
    };
    Utils.prototype.generateSwitchComponent = function (h, option, vm) {
        var _t = vm || this;
        var style = option.style, props = option.props, slot = option.slot, key = option.key, text = option.text;
        var on = translateEvents(option.events, _t);
        var compData = {
            directives: __spreadArrays((option.directives || []), [vEdit(_t)]),
            on: on,
            props: __assign(__assign({}, displayProp(_t)), props),
            style: style,
            slot: slot,
        };
        var vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
        return (<el-switch v-model={vmodelData[key]} {...compData}>
        {text}
      </el-switch>);
    };
    Utils.prototype.generateUploadComponent = function (h, option, vm) {
        var _t = vm || this;
        var style = option.style, _a = option.props, props = _a === void 0 ? {} : _a, directives = option.directives, key = option.key, events = option.events, label = option.label, mapKey = option.mapKey, _b = option.isFileDataById, isFileDataById = _b === void 0 ? false : _b;
        var slot = option.slot;
        var compData = {
            directives: directives,
            on: translateEvents(events || {}, _t),
            props: __assign(__assign(__assign({}, displayProp(_t)), { action: actionApi, disabled: _t.status === _t.$actionType.detail }), props),
            style: style
        };
        // 单个
        if (!props.multiple) {
            var imgID = _.get(_t.sourceFormData || _t.formData, key);
            compData.on["onBackImgId"] = function (event) {
                _.set(_t.sourceFormData || _t.formData, key, event);
            };
            return (<wtm-upload-img imgId={imgID} {...compData}>
          {option.children}
        </wtm-upload-img>);
        }
        // 上传钩子
        if (!compData.props.onSuccess) {
            compData.props.onSuccess = function (res, file, fileList) {
                var fileData = isFileDataById ? fileList.map(function (item) { return item.response ? item.response.Id : item.Id; }) : fileList;
                setMapKeyModel(_t, key, fileData, mapKey);
            };
        }
        // 删除钩子
        if (!compData.props.onRemove) {
            compData.props.onRemove = function (file, fileList) {
                var fileData = isFileDataById ? fileList.map(function (item) { return item.response ? item.response.Id : item.Id; }) : fileList;
                setMapKeyModel(_t, key, fileData, mapKey);
            };
        }
        if (isFileDataById) {
            // 赋值
            var value = getMapKeyModel(_t, key, mapKey);
            var dataFiles = [];
            if (value) {
                if (_.isArray(value)) {
                    dataFiles = value.map(function (item) { return ({ name: label, url: fileApi + item, Id: item }); });
                }
                else {
                    dataFiles = [{ name: label, url: fileApi + value, Id: value }];
                }
            }
            compData.props["file-list"] = dataFiles;
        }
        else {
            compData.props["file-list"] = sourceItem(_t.sourceFormData || _t.formData, key)[key] || [];
        }
        if (slot) {
            slot = slotRender(h, slot);
        }
        else {
            slot = <el-button type="primary">{_t.$t('form.clickUpload')}</el-button>;
        }
        return <el-upload {...compData}>{slot}</el-upload>;
    };
    Utils.prototype.generateWtmUploadImgComponent = function (h, option, vm) {
        var _t = vm || this;
        var style = option.style, props = option.props, slot = option.slot, directives = option.directives, key = option.key;
        var on = translateEvents(option.events, _t);
        on["onBackImgId"] = function (event) {
            _.set(_t.sourceFormData || _t.formData, key, event);
        };
        var compData = {
            directives: directives,
            on: on,
            props: __assign(__assign({}, displayProp(_t)), props),
            style: style,
            slot: slot,
        };
        var imgID = _.get(_t.sourceFormData || _t.formData, key);
        return (<wtm-upload-img imgId={imgID} {...compData}>
        {option.children}
      </wtm-upload-img>);
    };
    /**
     * 自定义位置
     * @param h
     * @param option
     * @param vm
     */
    Utils.prototype.generateWtmSlotComponent = function (h, option, vm) {
        var _t = vm || this;
        var key = option.key;
        if (option.components) {
            return option.components;
        }
        else {
            var data = {
                data: _.get(_t.sourceFormData || _t.formData, key),
                status: _t.status,
            };
            return _t.$scopedSlots[option.slotKey](data);
        }
    };
    Utils.prototype.generateLabelComponent = function (h, option, vm) {
        var _t = vm || this;
        var directives = option.directives, style = option.style, key = option.key;
        var on = translateEvents(option.events, _t);
        var compData = {
            directives: directives,
            on: on,
            style: style,
        };
        var value = _.get(_t.sourceFormData || _t.formData, key);
        return <label {...compData}>{value}</label>;
    };
    Utils.prototype.generateDatePickerComponent = function (h, option, vm) {
        var _t = vm || this;
        var directives = option.directives, props = option.props, style = option.style, key = option.key;
        if (!props) {
            props = {
                type: 'date',
                "value-format": "yyyy-MM-dd"
            };
        }
        var on = translateEvents(option.events, _t);
        var compData = {
            directives: __spreadArrays((directives || []), [vEdit(_t)]),
            on: on,
            props: __assign(__assign({}, displayProp(_t)), props),
            style: style,
        };
        var vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
        return (<el-date-picker v-model={vmodelData[key]} {...compData}></el-date-picker>);
    };
    Utils.prototype.generateTransferComponent = function (h, option, vm) {
        var _t = vm || this;
        var directives = option.directives, props = option.props, style = option.style, key = option.key, mapKey = option.mapKey;
        var on = __assign(__assign({}, translateEvents(option.events, _t)), { input: function (val) {
                setMapKeyModel(_t, key, val, mapKey);
            } });
        // 结构 Text，Value
        var editData = props.data.map(function (item) { return ({
            Text: item.label,
            Value: item.key,
        }); });
        var compData = {
            directives: __spreadArrays((directives || []), [vEdit(_t, editData)]),
            on: on,
            props: __assign(__assign({}, displayProp(_t)), props),
            style: style,
        };
        var value = getMapKeyModel(_t, key, mapKey);
        return <el-transfer value={value} {...compData}></el-transfer>;
    };
    return Utils;
}());
export default Utils;
/**
 * 事件
 * @param events
 * @param vm
 */
export var translateEvents = function (events, vm) {
    if (events === void 0) { events = {}; }
    var result = {};
    for (var event_1 in events) {
        result[event_1] = events[event_1].bind(vm);
    }
    return result;
};
/**
 * 编辑状态指令
 * @param vm
 * @param value 下拉框组件需要传入list
 */
export var vEdit = function (vm, value) {
    if (value === void 0) { value = null; }
    if (!vm.elDisabled) {
        return { name: "edit", arg: vm.status, value: value };
    }
    return {};
};
/**
 * display Prop
 * @param vm
 * @param value 下拉框组件需要传入list
 */
export var displayProp = function (vm) {
    if (vm.elDisabled) {
        return { disabled: vm.status === vm.$actionType.detail };
    }
    return {};
};
/**
 * 源数据 get,set
 * 支持：'Entity.ID' => Entity: { ID }
 * @param formData
 * @param keyPath
 */
export var sourceItem = function (formData, keyPath) {
    var _a;
    return _a = {},
        Object.defineProperty(_a, keyPath, {
            set: function (value) {
                _.set(formData, keyPath, value);
            },
            enumerable: false,
            configurable: true
        }),
        Object.defineProperty(_a, keyPath, {
            get: function () {
                return _.get(formData, keyPath);
            },
            enumerable: false,
            configurable: true
        }),
        _a;
};
/**
 * 转 render
 * @param hml
 * @param params
 */
export var slotRender = function (h, hml, params) {
    if (params === void 0) { params = {}; }
    var slot = undefined;
    if (_.isString(hml)) {
        slot = (<wtm-render-view hml={hml} params={__assign({}, params)}></wtm-render-view>);
    }
    return slot;
};
var setMapKeyModel = function (_t, key, value, mapKey) {
    var val = value;
    if (_.isArray(value) && mapKey) {
        val = value.map(function (item) {
            var _a;
            return (_a = {}, _a[mapKey] = item, _a);
        });
    }
    _.set(_t.sourceFormData || _t.formData, key, val);
};
var getMapKeyModel = function (_t, key, mapKey) {
    var valueList = _.get(_t.sourceFormData || _t.formData, key) || [];
    if (_.isArray(valueList)) {
        var val = valueList.map(function (item) { return mapKey ? item[mapKey] : item; });
        return val;
    }
    else {
        return valueList;
    }
};
//# sourceMappingURL=utils.jsx.map
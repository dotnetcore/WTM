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
export default class Utils {
  constructor() {}
  public wtmFormItem = this.generateWtmFormItemComponent;
  public input = this.generateInputComponent;
  public select = this.generateSelectComponent;
  public button = this.generateButtonComponent;
  public radio = this.generateRadioComponent;
  public radioGroup = this.generateRadioGroupComponent;
  public checkbox = this.generateCheckboxComponent;
  public checkboxGroup = this.generateCheckboxGroupComponent;
  public switch = this.generateSwitchComponent;
  public upload = this.generateUploadComponent;
  public wtmUploadImg = this.generateWtmUploadImgComponent;
  public wtmSlot = this.generateWtmSlotComponent;
  public label = this.generateLabelComponent;
  public datePicker = this.generateDatePickerComponent;
  public transfer = this.generateTransferComponent;

  /**
   * formItem 继承vue组件this
   * @param h
   * @param option
   * @param component
   * @param vm
   */
  private generateWtmFormItemComponent(h, option, component, vm?) {
    const _t = vm || this;
    const attrs = {
      label: _t.getLanguageByKey(option) + ":", // multi-language
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
    if (_t.status === _t.$actionType.detail) {
      const value = _.get(_t.sourceFormData || _t.formData, option.key);
      delete attrs.rules;
      // 图片
      if (option.type === "wtmUploadImg") {
        let img = !!value && (
          <el-image
            style={option.props.imageStyle}
            src={`/api/_file/downloadFile/${value}`}
            fit={option.props.fit || "contain"}
          />
        );
        return (
          <wtm-form-item ref={option.key} {...{ attrs, props: attrs }}>
            {img}
          </wtm-form-item>
        );
      }
    }
    return (
      <wtm-form-item ref={option.key} {...{ attrs, props: attrs }}>
        {component}
      </wtm-form-item>
    );
  }

  private generateInputComponent(h, option, vm?) {
    const _t = vm || this;
    const { style, props, slot, key } = option;
    const on = translateEvents(option.events, _t);
    const compData = {
      directives: [...(option.directives || []), vEdit(_t)],
      on,
      props: { ...displayProp(_t), ...props },
      style,
      slot,
    };
    let placeholder = `${_t.$t('form.pleaseEnter')}${option.label}`;
    if (props && props.placeholder) {
      placeholder = props.placeholder;
    }
    let vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
    return (
      <el-input
        v-model={vmodelData[key]}
        {...compData}
        placeholder={placeholder}
      >
        {slot}
      </el-input>
    );
  }

  private generateSelectComponent(h, option, vm?) {
    const _t = vm || this;
    const { style, props, key, mapKey } = option;
    let components = [];
    if (option.children && _.isArray(option.children)) {
      components = option.children.map((child) => {
        const value = child.Value;
        const label = child.Text || child.text;
        let slot: any = undefined;
        if (_.isString(child.slot)) {
          slot = (
            <wtm-render-view
              hml={child.slot}
              params={{ Value: value, Text: label }}
            ></wtm-render-view>
          );
        }
        return (
          <el-option
            key={child.Value}
            {...{ props: { value, label, ...child.props } }}
          >
            {slot}
          </el-option>
        );
      });
    }
    const compData = {
      directives: [...(option.directives || []), vEdit(_t, option.children)],
      on: translateEvents(option.events, _t),
      props: { ...displayProp(_t), ...props },
      style,
    };
    // mapkey && 多选
    if (mapKey && props.multiple) {
      compData.on["input"] = function (val) {
        _.set(
          _t.sourceFormData || _t.formData,
          key,
          val.map((item) => ({ [mapKey]: item }))
        );
      };
      const keyList = _.get(_t.sourceFormData || _t.formData, key) || [];
      const value = keyList.map(item => item[mapKey]);
      return (
        <el-select value={value} {...compData}>
          {components}
        </el-select>
      );
    } else {
      let vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
      return (
        <el-select v-model={vmodelData[key]} {...compData}>
          {components}
        </el-select>
      );
    }
  }

  private generateButtonComponent(h, option) {
    const { style, props, slot, events: on, text } = option;
    return <el-button {...{ style, props, slot, on }}>{text}</el-button>;
  }

  private generateRadioComponent(h, option, vm?) {
    const _t = vm || this;
    const { style, props, slot, key, text } = option;
    const on = translateEvents(option.events, _t);
    const compData = {
      directives: [...(option.directives || []), vEdit(_t)],
      on,
      props: { ...displayProp(_t), ...props },
      style,
      slot,
    };
    let vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
    return (
      <el-radio v-model={vmodelData[key]} {...compData}>
        {text}
      </el-radio>
    );
  }

  private generateRadioGroupComponent(h, option, vm?) {
    const _t = vm || this;
    const { style, props, slot, key } = option;
    let components = [];
    if (option.children) {
      components = option.children.map((child) => {
        const label = child.Value;
        const text = child.Text || child.text;
        return (
          <el-radio {...{ props: { ...child.props, label } }}>{text}</el-radio>
        );
      });
    }
    const on = translateEvents(option.events, _t);
    const compData = {
      directives: [...(option.directives || []), vEdit(_t, option.children)],
      on,
      props: { ...displayProp(_t), ...props },
      style,
      slot,
    };
    let vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
    return (
      <el-radio-group v-model={vmodelData[key]} {...compData}>
        {components}
      </el-radio-group>
    );
  }

  private generateCheckboxComponent(h, option, vm?) {
    const _t = vm || this;
    const { style, props, slot, key, text } = option;
    const on = translateEvents(option.events, _t);
    const compData = {
      directives: [...(option.directives || []), vEdit(_t)],
      on,
      props: { ...displayProp(_t), ...props },
      style,
      slot,
    };
    let vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
    return (
      <el-checkbox v-model={vmodelData[key]} {...compData}>
        {text}
      </el-checkbox>
    );
  }

  private generateCheckboxGroupComponent(h, option, vm?) {
    const _t = vm || this;
    const { style, props, slot, key, mapKey } = option;
    let components = [];
    if (option.children) {
      components = option.children.map((child) => {
        const label = child.Value;
        const text = child.Text || child.text;
        return (
          <el-checkbox {...{ props: { ...child.props, label } }}>
            {text}
          </el-checkbox>
        );
      });
    }
    const on = translateEvents(option.events, _t);
    const compData = {
      directives: [...(option.directives || []), vEdit(_t)],
      on,
      props: { ...displayProp(_t), ...props },
      style,
      slot,
    };
    if (mapKey) {
      compData.on["input"] = function (val) {
        _.set(
          _t.sourceFormData || _t.formData,
          key,
          val.map((item) => ({ [mapKey]: item }))
        );
      };
      // const value = _.get(_t.sourceFormData || _t.formData, key)[mapKey];
      const keyList = _.get(_t.sourceFormData || _t.formData, key) || [];
      const value = keyList.map(item => item[mapKey]);
      return (
        <el-checkbox-group value={value} {...compData}>
          {components}
        </el-checkbox-group>
      );
    } else {
      let vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
      return (
        <el-checkbox-group v-model={vmodelData[key]} {...compData}>
          {components}
        </el-checkbox-group>
      );
    }
  }

  private generateSwitchComponent(h, option, vm?) {
    const _t = vm || this;
    const { style, props, slot, key, text } = option;
    const on = translateEvents(option.events, _t);
    const compData = {
      directives: [...(option.directives || []), vEdit(_t)],
      on,
      props: { ...displayProp(_t), ...props },
      style,
      slot,
    };
    let vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
    return (
      <el-switch v-model={vmodelData[key]} {...compData}>
        {text}
      </el-switch>
    );
  }

  private generateUploadComponent(h, option, vm?) {
    const _t = vm || this;
    const { style, props, slot, directives, key, events, label } = option;
    const actionApi = "/api/_file/upload";
    const fileApi = "/api/_file/downloadFile/";
    const compData = {
      directives,
      on: events || {},
      props: { ...displayProp(_t), ...props },
      style,
    };
    compData.props["file-list"] = [];
    compData.props.limit = 1;
    if (!compData.props.action) {
      compData.props.action = actionApi;
    }
    if (!compData.props.onSuccess) {
      compData.props.onSuccess = (res, file) => {
        // _t.formData[key] = res.Id;
        _.set(_t.sourceFormData || _t.formData, key, res.Id);
      };
    }
    const imgID = _.get(_t.sourceFormData || _t.formData, key);
    if (imgID) {
      compData.props["file-list"] = [{ name: label, url: fileApi + imgID }];
    }
    const defaultSlot = <el-button type="primary">{_t.$t('form.clickUpload')}</el-button>;
    return <el-upload {...compData}>{slot || defaultSlot}</el-upload>;
  }

  private generateWtmUploadImgComponent(h, option, vm?) {
    const _t = vm || this;
    const { style, props, slot, directives, key } = option;
    const on = translateEvents(option.events, _t);
    on["onBackImgId"] = (event) => {
      _.set(_t.sourceFormData || _t.formData, key, event);
    };
    const compData = {
      directives,
      on,
      props: { ...displayProp(_t), ...props },
      style,
      slot,
    };
    const imgID = _.get(_t.sourceFormData || _t.formData, key);
    return (
      <wtm-upload-img imgId={imgID} {...compData}>
        {option.children}
      </wtm-upload-img>
    );
  }
  /**
   * 自定义位置
   * @param h
   * @param option
   * @param vm
   */
  private generateWtmSlotComponent(h, option, vm?) {
    const _t = vm || this;
    const { key } = option;
    if (option.components) {
      return option.components;
    } else {
      const data = {
        data: _.get(_t.sourceFormData || _t.formData, key),
        status: _t.status,
      };
      return _t.$scopedSlots[option.slotKey](data);
    }
  }

  private generateLabelComponent(h, option, vm?) {
    const _t = vm || this;
    const { directives, style, key } = option;
    const on = translateEvents(option.events, _t);
    const compData = {
      directives,
      on,
      style,
    };
    const value = _.get(_t.sourceFormData || _t.formData, key);
    return <label {...compData}>{value}</label>;
  }

  private generateDatePickerComponent(h, option, vm?) {
    const _t = vm || this;
    const { directives, props, style, key } = option;
    const on = translateEvents(option.events, _t);
    const compData = {
      directives: [...(directives || []), vEdit(_t)],
      on,
      props: { ...displayProp(_t), ...props },
      style,
    };
    let vmodelData = sourceItem(_t.sourceFormData || _t.formData, key);
    return (
      <el-date-picker v-model={vmodelData[key]} {...compData}></el-date-picker>
    );
  }

  private generateTransferComponent(h, option, vm?) {
    const _t = vm || this;
    const { directives, props, style, key, mapKey } = option;
    const on = {
      ...translateEvents(option.events, _t),
      input: function (val) {
        const value = val.map((item) => (mapKey ? { [mapKey]: item } : item));
        _.set(_t.sourceFormData || _t.formData, key, value);
      },
    };
    // 结构 Text，Value
    const editData = props.data.map((item) => ({
      Text: item.label,
      Value: item.key,
    }));
    const compData = {
      directives: [...(directives || []), vEdit(_t, editData)],
      on,
      props: { ...displayProp(_t), ...props },
      style,
    };
    // _t.formData[key]
    const value = _.get(_t.sourceFormData || _t.formData, key).map((item) =>
      mapKey ? item[mapKey] : item
    );
    return <el-transfer value={value} {...compData}></el-transfer>;
  }
}

/**
 * 事件
 * @param events
 * @param vm
 */
export const translateEvents = (events = {}, vm) => {
  const result = {};
  for (let event in events) {
    result[event] = events[event].bind(vm);
  }
  return result;
};
/**
 * 编辑状态指令
 * @param vm
 * @param value 下拉框组件需要传入list
 */
export const vEdit = (vm, value: Array<any> | null = null) => {
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
export const displayProp = (vm) => {
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
export const sourceItem = (formData, keyPath) => {
  return {
    set [keyPath](value) {
      _.set(formData, keyPath, value);
    },
    get [keyPath]() {
      return _.get(formData, keyPath);
    },
  };
};
// function fn(obj: object, path: string[]) {
//   if (path.length > 1) {
//     const key = path.shift();
//     return key !== undefined ? fn(obj[key], path) : obj;
//   } else {
//     return obj;
//   }
// }

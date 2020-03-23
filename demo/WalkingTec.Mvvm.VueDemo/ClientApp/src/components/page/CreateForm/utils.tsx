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
      label: option.label,
      rules: option.rules,
      prop: option.key ? option.key : "",
      error: option.error,
      "label-width": option["label-width"] || option["labelWidth"],
      span: option.span,
      isShow: option.isShow,
      value: option.value,
      isImg: option.isImg
    };
    // 展示状态 需要操作的组件
    if (_t.status === _t.$actionType.detail) {
      const value = _t.formData[option.key];
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
      props,
      style,
      slot
    };
    let placeholder = `请输入${option.label}`;
    if (props && props.placeholder) {
      placeholder = props.placeholder;
    }
    return (
      <el-input
        v-model={_t.formData[key]}
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
      components = option.children.map(child => {
        const value = child.Value;
        const label = child.Text || child.text;
        const slot = child.slot;
        return (
          <el-option {...{ props: { value, label, ...child.props } }}>
            {slot}
          </el-option>
        );
      });
    }
    const compData = {
      directives: [...(option.directives || []), vEdit(_t, option.children)],
      on: translateEvents(option.events, _t),
      props,
      style
    };
    // mapkey && 多选
    if (mapKey && props.multiple) {
      compData.on["input"] = function(val) {
        _t.formData[key] = val.map(item => ({ [mapKey]: item }));
      };
      const value = _t.formData[key].map(item => item[mapKey]);
      return (
        <el-select value={value} {...compData}>
          {components}
        </el-select>
      );
    } else {
      return (
        <el-select v-model={_t.formData[key]} {...compData}>
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
      props,
      style,
      slot
    };
    return (
      <el-radio v-model={_t.formData[key]} {...compData}>
        {text}
      </el-radio>
    );
  }

  private generateRadioGroupComponent(h, option, vm?) {
    const _t = vm || this;
    const { style, props, slot, key } = option;
    let components = [];
    if (option.children) {
      components = option.children.map(child => {
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
      props,
      style,
      slot
    };
    return (
      <el-radio-group v-model={_t.formData[key]} {...compData}>
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
      props,
      style,
      slot
    };
    return (
      <el-checkbox v-model={_t.formData[key]} {...compData}>
        {text}
      </el-checkbox>
    );
  }

  private generateCheckboxGroupComponent(h, option, vm?) {
    const _t = vm || this;
    const { style, props, slot, key, mapKey } = option;
    let components = [];
    if (option.children) {
      components = option.children.map(child => {
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
      props,
      style,
      slot
    };
    if (mapKey) {
      compData.on["input"] = function(val) {
        _t.formData[key] = val.map(item => ({ [mapKey]: item }));
      };
      const value = _t.formData[key].map(item => item[mapKey]);
      return (
        <el-checkbox-group value={value} {...compData}>
          {components}
        </el-checkbox-group>
      );
    } else {
      return (
        <el-checkbox-group v-model={_t.formData[key]} {...compData}>
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
      props,
      style,
      slot
    };
    return (
      <el-switch v-model={_t.formData[key]} {...compData}>
        {text}
      </el-switch>
    );
  }

  private generateUploadComponent(h, option, vm?) {
    const _t = vm || this;
    const { style, props, slot, directives, key, events } = option;
    const actionApi = "/api/_file/upload";
    const fileApi = "/api/_file/downloadFile/";
    const compData = {
      directives,
      on: events || {},
      props: props || {},
      style
    };
    compData.props["file-list"] = [];
    compData.props.limit = 1;
    if (!compData.props.action) {
      compData.props.action = actionApi;
    }
    if (!compData.props.onSuccess) {
      compData.props.onSuccess = (res, file) => {
        _t.formData[key] = res.Id;
      };
    }
    if (_t.formData[key]) {
      compData.props["file-list"] = [
        { name: "defalut", url: fileApi + _t.formData[key] }
      ];
    }
    const defaultSlot = <el-button type="primary">点击上传</el-button>;
    return <el-upload {...compData}>{slot || defaultSlot}</el-upload>;
  }

  private generateWtmUploadImgComponent(h, option, vm?) {
    const _t = vm || this;
    const { style, props, slot, directives, key } = option;
    const on = translateEvents(option.events, _t);
    on["onBackImgId"] = event => {
      _t.formData[key] = event;
    };
    const compData = {
      directives,
      on,
      props,
      style,
      slot
    };
    return (
      <wtm-upload-img imgId={_t.formData[key]} {...compData}>
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
      const data = { data: _t.formData[key], status: _t.status };
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
      style
    };
    return <label {...compData}>{_t.formData[key]}</label>;
  }

  private generateDatePickerComponent(h, option, vm?) {
    const _t = vm || this;
    const { directives, props, style, key } = option;
    const on = translateEvents(option.events, _t);
    const compData = {
      directives: [...(directives || []), vEdit(_t)],
      on,
      props,
      style
    };
    return (
      <el-date-picker v-model={_t.formData[key]} {...compData}></el-date-picker>
    );
  }

  private generateTransferComponent(h, option, vm?) {
    const _t = vm || this;
    const { directives, props, style, key, mapKey } = option;
    const on = {
      ...translateEvents(option.events, _t),
      input: function(val) {
        _t.formData[key] = val.map(item =>
          mapKey ? { [mapKey]: item } : item
        );
      }
    };
    // 结构 Text，Value
    const editData = props.data.map(item => ({
      Text: item.label,
      Value: item.key
    }));
    const compData = {
      directives: [...(directives || []), vEdit(_t, editData)],
      on,
      props,
      style
    };
    const value = _t.formData[key].map(item => (mapKey ? item[mapKey] : item));
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
  return { name: "edit", arg: vm.status, value: value };
};

/**
 * 需要继承vue的this
 *
 * ['WtmFormItem']
 * /vue-custom/component/index
 *
 * 定义组件 注：
 * model使用
 *    <el-input v-model={formData[key]} />
 * value使用
 *    const on = {
 *      input(val) {
 *        formData[key] = val;
 *      }
 *    };
 *    <el-input value={formData[key]} ...{on}/>
 */
const componentObj = {
  input: generateInputComponent,
  select: generateSelectComponent,
  button: generateButtonComponent,
  radio: generateRadioComponent,
  radioGroup: generateRadioGroupComponent,
  checkbox: generateCheckboxComponent,
  checkboxGroup: generateCheckboxGroupComponent,
  switch: generateSwitchComponent,
  wtmformItem: generateWtmFormItemComponent
};
/**
 * 事件
 * @param events
 * @param vm
 */
function translateEvents(events = {}, vm) {
  const result = {};
  for (let event in events) {
    result[event] = events[event].bind(vm);
  }

  return result;
}
/**
 * 编辑状态指令
 * @param vm
 * @param value 下拉框组件需要传入list
 */
const vEdit = (vm, value: Array<any> | null = null) => {
  return { name: "edit", arg: vm.status, value: value };
};

function generateInputComponent(h, formData = {}, option, vm) {
  const { style, props: attrs, slot, key } = option;
  const directives = [...(option.directives || []), vEdit(vm)];
  const on = translateEvents(option.events, vm);
  return (
    <el-input v-model={formData[key]} {...{ directives, on, attrs, style }}>
      {slot}
    </el-input>
  );
}

function generateSelectComponent(h, formData = {}, option, vm) {
  const { style, props: attrs, slot, key } = option;
  let components = [];
  if (option.children) {
    components = option.children.map(child => {
      return <el-option {...{ attrs: child.props }}></el-option>;
    });
  }
  const on = translateEvents(option.events, vm);
  const compData = {
    directives: [...(option.directives || []), vEdit(vm)],
    on,
    attrs,
    style,
    slot
  };
  return (
    <el-select v-model={formData[key]} {...compData}>
      {components}
    </el-select>
  );
}

function generateButtonComponent(h, formData = {}, option) {
  const { style, props: attrs, slot, events: on, text } = option;
  return <el-button {...{ style, attrs, slot, on }}>{text}</el-button>;
}

function generateRadioComponent(h, formData = {}, option, vm) {
  const { style, props: attrs, slot, key, text } = option;
  const on = translateEvents(option.events, vm);
  const compData = {
    directives: [...(option.directives || []), vEdit(vm)],
    on,
    attrs,
    style,
    slot
  };
  return (
    <el-radio v-model={formData[key]} {...compData}>
      {text}
    </el-radio>
  );
}

function generateRadioGroupComponent(h, formData = {}, option, vm) {
  const { style, props: attrs, slot, key } = option;
  let components = [];
  if (option.children) {
    components = option.children.map(child => {
      return <el-radio {...{ attrs: child.props }}>{child.text}</el-radio>;
    });
  }
  const on = translateEvents(option.events, vm);
  const compData = {
    directives: [...(option.directives || []), vEdit(vm)],
    on,
    attrs,
    style,
    slot
  };
  return (
    <el-radio-group v-model={formData[key]} {...compData}>
      {components}
    </el-radio-group>
  );
}

function generateCheckboxComponent(h, formData = {}, option, vm) {
  const { style, props: attrs, slot, key, text } = option;
  const on = translateEvents(option.events, vm);
  const compData = {
    directives: [...(option.directives || []), vEdit(vm)],
    on,
    attrs,
    style,
    slot
  };
  return (
    <el-checkbox v-model={formData[key]} {...compData}>
      {text}
    </el-checkbox>
  );
}

function generateCheckboxGroupComponent(h, formData = {}, option, vm) {
  const { style, props: attrs, slot, key } = option;
  let components = [];
  if (option.children) {
    components = option.children.map(child => {
      return (
        <el-checkbox {...{ attrs: child.props }}>{child.text}</el-checkbox>
      );
    });
  }
  const on = translateEvents(option.events, vm);
  const compData = {
    directives: [...(option.directives || []), vEdit(vm)],
    on,
    attrs,
    style,
    slot
  };
  return (
    <el-checkbox-group v-model={formData[key]} {...compData}>
      {components}
    </el-checkbox-group>
  );
}

function generateSwitchComponent(h, formData = {}, option, vm) {
  const { style, props: attrs, slot, key, text } = option;
  const on = translateEvents(option.events, vm);
  const compData = {
    directives: [...(option.directives || []), vEdit(vm)],
    on,
    attrs,
    style,
    slot
  };
  return (
    <el-switch v-model={formData[key]} {...compData}>
      {text}
    </el-switch>
  );
}

function generateWtmFormItemComponent(h, option, component) {
  const attrs = {
    label: option.label,
    rules: option.rules,
    prop: option.key ? option.key : "",
    error: option.error,
    "label-width": option["label-width"] || option["labelWidth"]
  };
  return <wtm-form-item {...{ attrs }}>{component}</wtm-form-item>;
}

export default componentObj;

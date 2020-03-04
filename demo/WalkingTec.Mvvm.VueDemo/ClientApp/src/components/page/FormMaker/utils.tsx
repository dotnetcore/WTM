/**
 * 需要继承vue的this
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
  button: generateButtonComponent,
  radio: generateRadioComponent,
  radioGroup: generateRadioGroupComponent,
  checkbox: generateCheckboxComponent,
  checkboxGroup: generateCheckboxGroupComponent,
  switch: generateSwitchComponent,
  select: generateSelectComponent
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
  const { style, props, slot, key } = option;
  const directives = [...(option.directives || []), vEdit(vm)];
  const on = {
    ...translateEvents(option.events, vm)
  };
  return (
    <el-input v-model={formData[key]} {...{ directives, on, props, style }}>
      {slot}
    </el-input>
  );
}
function generateButtonComponent(h, formData = {}, option) {
  const { style, props, slot, events: on, text } = option;
  return <el-button {...{ style, props, slot, on }}>{text}</el-button>;
}

function generateRadioComponent(h, formData = {}, option, vm) {
  const { style, props, slot, key, text } = option;
  const on = {
    ...translateEvents(option.events, vm)
  };
  const compData = {
    directives: [...(option.directives || []), vEdit(vm)],
    on,
    props,
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
  const { style, props, slot, key } = option;
  let components = [];
  if (option.children) {
    components = option.children.map(child => {
      return <el-radio {...{ props: child.props }}>{child.text}</el-radio>;
    });
  }
  const on = {
    ...translateEvents(option.events, vm)
  };
  const compData = {
    directives: [...(option.directives || []), vEdit(vm)],
    on,
    props,
    style,
    slot
  };

  return (
    <el-radio-group v-model={formData[key]} {...compData}>
      {components}
    </el-radio-group>
  );
}

function generateCheckboxComponent() {}
function generateCheckboxGroupComponent() {}
function generateSwitchComponent() {}
function generateSelectComponent() {}

export default componentObj;

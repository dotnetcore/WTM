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

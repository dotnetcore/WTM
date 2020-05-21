import { DirectiveOptions } from "vue";
/**
 * 组件状态展示 编辑/文本
 *
 * 组件类型 使用：
 * 1. 文本字段
 *    v-edit:[status]
 *    status： 增/删/改 状态
 * 2. 集合字段
 *    v-edit:[status]="list"
 *    list:Array<any> 过滤集合,格式 [{Value,Text}]
 *    status： 增/删/改 状态
 *
 */
const domStyleFn = (el, { value: sourceVal, arg }, vnode) => {
  // const { value, arg } = binding; // value Array<any>
  let modelVal = "";
  if (vnode.data.model) {
    modelVal = vnode.data.model.value; // v-model 值
  } else if (vnode.componentOptions.propsData) {
    modelVal = vnode.componentOptions.propsData.value; // value 值
  } else {
    console.error(`dirEdit指令接收参数：v-model或value错误`);
  }
  // 内容value
  let htmlVal: string = modelVal;
  // el前dom，insert添加的dom
  let elPre = el.previousSibling || {};
  // 添加&修改 展示组件
  if (arg === "add" || arg === "edit") {
    el.style.display = "";
    elPre.style.display = "none";
    return;
  }
  // 判断
  const sourceArr = _.isArray(sourceVal);
  const sourceObj = _.isPlainObject(sourceVal);
  const valArr = _.isArray(modelVal);
  const valStr = _.isString(modelVal);
  const valNum = _.isNumber(modelVal);
  const valBoo = _.isBoolean(modelVal);
  try {
    if (sourceArr && valArr) {
      htmlVal = sourceVal
        .filter(res => modelVal.includes(res["Value"]))
        .map(item => item.Text)
        .join(",");
    } else if (sourceArr && (valStr || valNum || valBoo)) {
      const item = _.find(sourceVal, { Value: modelVal });
      htmlVal = item ? item["Text"] : modelVal;
    } else if (sourceObj && valStr) {
      htmlVal = sourceVal[modelVal];
    } else if (!sourceVal && valBoo) {
      htmlVal = modelVal ? "是" : "否";
      return;
    }
  } catch (error) {
    console.warn(`dirEdit,指令接受的'${modelVal}'结构不符合要求`, error);
    htmlVal = modelVal;
  }
  el.style.display = "none";
  elPre.innerHTML = htmlVal;
  elPre.style.display = "inline-block";
};
// 编辑
const edit: DirectiveOptions = {
  inserted: (el, { value, arg, expression }, vnode) => {
    // 组件前添加dom
    let spanDom = el.previousSibling;
    if (!spanDom) {
      spanDom = document.createElement("span");
      spanDom.style.display = "none";
      el.parentNode && el.parentNode.insertBefore(spanDom, el);
    }
    domStyleFn(el, { value, arg }, vnode);
  },
  update: (el, { value, arg }, vnode) => domStyleFn(el, { value, arg }, vnode)
};

export default edit;

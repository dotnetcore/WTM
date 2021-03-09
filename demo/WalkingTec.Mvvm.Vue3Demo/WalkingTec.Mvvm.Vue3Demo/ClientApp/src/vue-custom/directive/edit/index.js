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
var domStyleFn = function (el, _a, vnode) {
    var sourceVal = _a.value, arg = _a.arg;
    // const { value, arg } = binding; // value Array<any>
    var modelVal = "";
    if (vnode.data.model) {
        modelVal = vnode.data.model.value; // v-model 值
    }
    else if (vnode.componentOptions.propsData) {
        modelVal = vnode.componentOptions.propsData.value; // value 值
    }
    else {
        console.error("dirEdit\u6307\u4EE4\u63A5\u6536\u53C2\u6570\uFF1Av-model\u6216value\u9519\u8BEF");
    }
    // 内容value
    var htmlVal = modelVal || '';
    // el前dom，insert添加的dom
    var elPre = el.previousSibling || {};
    // 添加&修改 展示组件
    if (arg === "add" || arg === "edit") {
        el.style.display = "";
        elPre.style.display = "none";
        return;
    }
    // 判断
    var sourceArr = _.isArray(sourceVal);
    var sourceObj = _.isPlainObject(sourceVal);
    var valArr = _.isArray(modelVal);
    var valStr = _.isString(modelVal);
    var valNum = _.isNumber(modelVal);
    var valBoo = _.isBoolean(modelVal);
    try {
        if (sourceArr && valArr) {
            htmlVal = sourceVal
                .filter(function (res) { return modelVal.includes(res["Value"]); })
                .map(function (item) { return item.Text; })
                .join(",");
        }
        else if (sourceArr && (valStr || valNum || valBoo)) {
            var item = _.find(sourceVal, { Value: modelVal });
            htmlVal = item ? item["Text"] : modelVal;
        }
        else if (sourceObj && valStr) {
            htmlVal = sourceVal[modelVal];
        }
        else if (!sourceVal && valBoo) {
            htmlVal = modelVal ? "是" : "否";
            return;
        }
    }
    catch (error) {
        console.warn("dirEdit,\u6307\u4EE4\u63A5\u53D7\u7684'" + modelVal + "'\u7ED3\u6784\u4E0D\u7B26\u5408\u8981\u6C42", error);
        htmlVal = modelVal;
    }
    el.style.display = "none";
    elPre.innerHTML = htmlVal;
    elPre.style.display = "inline-block";
};
// 编辑
var edit = {
    inserted: function (el, _a, vnode) {
        var value = _a.value, arg = _a.arg, expression = _a.expression;
        // 组件前添加dom
        var spanDom = el.previousSibling;
        if (!spanDom) {
            spanDom = document.createElement("span");
            spanDom.style.display = "none";
            el.parentNode && el.parentNode.insertBefore(spanDom, el);
        }
        domStyleFn(el, { value: value, arg: arg }, vnode);
    },
    update: function (el, _a, vnode) {
        var value = _a.value, arg = _a.arg;
        return domStyleFn(el, { value: value, arg: arg }, vnode);
    }
};
export default edit;
//# sourceMappingURL=index.js.map
/**
 * 1. 文本字段
 * dirEdit 指令-是否编辑
 * v-edit:[status]
 * status： 增/删/改 状态
 * 2. 集合字段
 * v-edit:[status]="{list: Array<any>, key: string, label: string}"
 * list 过滤集合
 * key 集合中v-model对应字段名
 * label 集合中 需要展示的字段名
 */
const domStyleFn = (el, binding, vnode) => {
    const { value, arg } = binding; // value {list: Array<any>, key: string, label: string}
    const modelVal = vnode.data.model.value; // v-model 值
    let htmlVal: string = modelVal; // 内容value
    let elPre = el.previousSibling || {}; // el前dom，insert添加的dom
    if (value && modelVal) {
        try {
            const { list, key, label } = value;
            // 参数判断
            htmlVal = list.filter(res => res[key] === modelVal)[0][label];
        } catch (error) {
            console.log("value", value, modelVal);
            console.error("结构不符合要求", error);
        }
    }
    // 添加&修改 展示组件
    if (arg === "add" || arg === "edit") {
        el.style.display = "inline-block";
        elPre.style.display = "none";
    } else {
        el.style.display = "none";
        elPre.innerHTML = htmlVal;
        elPre.style.display = "inline-block";
    }
};

export const dirEdit = {
    inserted: (el, binding, vnode) => {
        // el前添加dom
        const div = document.createElement("div");
        div.innerHTML = `<span id="${
            binding.expression
        }" style="display: none;"></span>`;
        el.parentNode && el.parentNode.insertBefore(div.childNodes[0], el);
        domStyleFn(el, binding, vnode);
    },
    update: (el, binding, vnode) => domStyleFn(el, binding, vnode)
};
// // 按钮action权限
// export const visible = {
//     inserted: (el, binding, vnode) => {
//         binding.value
//     }
// }

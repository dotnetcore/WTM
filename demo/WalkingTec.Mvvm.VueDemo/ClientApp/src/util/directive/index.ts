// import user from "@/store/common/user";
import { UserModule } from "@/store/modules/user";
import config from "@/config/index";
/**
 * 1. 文本字段
 * edit 指令-是否编辑
 * v-edit:[status]
 * status： 增/删/改 状态
 * 2. 集合字段
 * v-edit:[status]="{list: Array<any>, key: string, label: string}"
 * list 过滤集合
 * key 集合中v-model对应字段名
 * label 集合中 需要展示的字段名
 */
const domStyleFn = (el, { value, arg }, vnode) => {
    // const { value, arg } = binding; // value {list: Array<any>, key: string, label: string}
    const modelVal = vnode.data.model.value; // v-model 值
    let htmlVal: string = modelVal; // 内容value
    let elPre = el.previousSibling || {}; // el前dom，insert添加的dom
    if (value && modelVal) {
        try {
            const { list, key, label } = value;
            // 参数判断
            htmlVal = list.filter(res => res[key] === modelVal)[0][label];
        } catch (error) {
            console.warn("dirEdit,指令结构不符合要求", error);
            htmlVal = modelVal;
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
// 编辑
export const edit = {
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
// 按钮action权限
export const visible = {
    inserted: (el, binding, vnode) => {
        if (config.development) {
            return;
        }
        if (_.isArray(binding.value)) {
            const fslist = _.filter(binding.value, item =>
                UserModule.actionList.includes(item)
            );
            if (fslist.length < 1) {
                el.parentNode.removeChild(el);
            }
        } else {
            if (!UserModule.actionList.includes(_.toLower(binding.value))) {
                el.parentNode.removeChild(el);
            }
        }
    }
};
// error提示
export const error = {
    update: (el, binding, vnode) => {
        console.log("---------------------------");
        console.log(el, binding, vnode);
    }
};
export * from "./el-draggable-dialog";

const domStyleFn = (el, binding, vnode?) => {
    console.log("binding", binding, vnode);
    const { modifiers, value, arg } = binding;
    const modelVal = vnode.data.model.value;
    let htmlVal: string = modelVal; // 内容value
    let elPre = el.previousSibling || {}; // el前dom，insert添加的dom
    // let assembly: Array<any> = []; // el如果为下拉框 集合
    // let raws: Array<string> = []; // 获取string参数,获取指令全名称name[status(增删改)，list(如果查询结果为列表，传入列表数据)]
    // let assemblyName: string = ""; // el如果为下拉框集合 在list中的名称
    // raws = binding["rawName"].match(/\[([^)]*)\]/) || []; // 获取名称中括号中的内容
    // assemblyName = raws[1].split(",")[1];
    // assembly = binding[assemblyName];
    if (value) {
        try {
            const { list, key, label } = value;
            // 参数判断
            htmlVal = list.filter(res => res[key] === modelVal)[0][label];
        } catch (error) {
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

// 大写首字母
const firstUpperCase = str => {
    return str.replace(/( |^)[a-z]/g, L => L.toUpperCase());
};
// params 返回 string
const urlStringify = params => {
    const list = Object.keys(params)
        .map(item => {
            return item + '=' + params[item];
        })
        .join('&');
    return list;
};
// params 返回 string
const strStringify = (params, seg, segjoin) => {
    const list = Object.keys(params)
        .map(item => {
            return item + (seg || '=') + params[item];
        })
        .join(segjoin || '&');
    return list;
};
export { firstUpperCase, urlStringify, strStringify };

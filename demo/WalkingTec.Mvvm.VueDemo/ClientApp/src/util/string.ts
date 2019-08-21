// 大写首字母
const firstUpperCase = str => {
    return str.replace(/( |^)[a-z]/g, L => L.toUpperCase());
};
// params 返回 string
const urlStringify = params => {
    const list = Object.keys(params)
        .map(item => {
            return item + "=" + params[item];
        })
        .join("&");
    return list;
};
// params 返回 string
const strStringify = (params, seg, segjoin) => {
    const list = Object.keys(params)
        .map(item => {
            return item + (seg || "=") + params[item];
        })
        .join(segjoin || "&");
    return list;
};
// 图片格式data
const getBase64Str = obj => {
    const u8arr = new Uint8Array(obj).reduce(
        (data, byte) => data + String.fromCharCode(byte),
        ""
    );
    return "data:image/png;base64," + btoa(u8arr);
};
// 图片转换Blob
const getDownLoadUrl = base64str => {
    let parts = base64str.split(";base64,");
    let contentType = parts[0].split(":")[1];
    let raw = window.atob(parts[1]);
    let rawLength = raw.length;
    let uInt8Array = new Uint8Array(rawLength);
    for (let i = 0; i < rawLength; ++i) {
        uInt8Array[i] = raw.charCodeAt(i);
    }
    const blob = new Blob([uInt8Array], { type: contentType });
    return URL.createObjectURL(blob);
};

const listToString = (list: Array<any>, key: string) => {
    list.map(item => {
        return item[key];
    }).join(",");
};
export {
    firstUpperCase,
    urlStringify,
    strStringify,
    getBase64Str,
    getDownLoadUrl,
    listToString
};

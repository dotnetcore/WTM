// 大写首字母
const firstUpperCase = str => {
  // 转换字符串首字母为大写，剩下为小写。
  //_.capitalize([string=''])
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

const listToString = (list: Array<any> = [], key: string) => {
  return list.map(item => {
    return item[key];
  });
};

const exportXlsx = (data, name) => {
  let b = new Blob([data], {
    type: "application/vnd.ms-excel;charset=utf-8"
  });
  let url = URL.createObjectURL(b);
  let link = document.createElement("a");
  link.download = name + ".xls";
  link.href = url;
  link.click();
  window.URL.revokeObjectURL(url); //释放掉blob对象
};
/**
 * 替换模版{key}
 */
const paramTemplate = (str, param) => {
  if (
    _.isObject(param) &&
    typeof param === "object" &&
    /{([\s\S]+?)}/g.test(str)
  ) {
    str = _.template(str, { interpolate: /{([\s\S]+?)}/g })(param);
  }
  return str;
};

export {
  firstUpperCase,
  urlStringify,
  strStringify,
  getBase64Str,
  getDownLoadUrl,
  listToString,
  exportXlsx,
  paramTemplate
};

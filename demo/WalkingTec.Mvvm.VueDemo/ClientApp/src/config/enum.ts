// 请求类型
export enum contentType {
  form = "application/x-www-form-urlencoded; charset=UTF-8",
  json = "application/json",
  multipart = "multipart/form-data",
  stream = "application/json",
  arraybuffer = "arraybuffer" // 图片buffer
}
// 弹出框类型 actionType
export enum actionType {
  detail = "detail",
  edit = "edit",
  add = "add"
}
// 按钮类型
export enum butType {
  edit = "edit",
  add = "add",
  delete = "delete",
  deleted = "delete",
  imported = "imported",
  export = "export"
}

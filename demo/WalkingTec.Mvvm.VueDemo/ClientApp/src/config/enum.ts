// 请求类型
export enum contentType {
    form = "application/x-www-form-urlencoded; charset=UTF-8",
    json = "application/json",
    multipart = "multipart/form-data",
    stream = "application/json"
}
// 弹出框类型
export enum dialogType {
    detail = "detail",
    edit = "edit",
    add = "add"
}
// 按钮类型
export enum butType {
    edit = "edit",
    add = "add",
    delete = "delete",
    import = "import",
    export = "export"
}

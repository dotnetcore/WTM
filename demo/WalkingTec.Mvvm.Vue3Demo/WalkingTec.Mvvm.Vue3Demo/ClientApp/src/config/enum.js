// 请求类型
export var contentType;
(function (contentType) {
    contentType["form"] = "application/x-www-form-urlencoded; charset=UTF-8";
    contentType["json"] = "application/json";
    contentType["multipart"] = "multipart/form-data";
    contentType["stream"] = "application/json";
    contentType["arraybuffer"] = "arraybuffer"; // 图片buffer
})(contentType || (contentType = {}));
// 弹出框类型 actionType
export var actionType;
(function (actionType) {
    actionType["detail"] = "detail";
    actionType["edit"] = "edit";
    actionType["add"] = "add";
})(actionType || (actionType = {}));
// 按钮类型
export var butType;
(function (butType) {
    butType["edit"] = "edit";
    butType["add"] = "add";
    butType["delete"] = "delete";
    butType["deleted"] = "delete";
    butType["imported"] = "imported";
    butType["export"] = "export";
})(butType || (butType = {}));
//# sourceMappingURL=enum.js.map
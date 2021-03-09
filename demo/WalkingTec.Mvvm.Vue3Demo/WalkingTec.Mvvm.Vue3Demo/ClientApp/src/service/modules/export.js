/**
 *  导出
 * /admin/role
 */
// 获取session
var getExportInfoService = {
    url: "/export",
    method: "post"
};
// 获取状态
var getProgressService = {
    url: "/export/progresses/{session}",
    method: "get"
};
// 获取下载地址
var getDownloadInfo = {
    url: "/export/download/{session}",
    method: "get"
};
export { getExportInfoService, getProgressService, getDownloadInfo };
//# sourceMappingURL=export.js.map
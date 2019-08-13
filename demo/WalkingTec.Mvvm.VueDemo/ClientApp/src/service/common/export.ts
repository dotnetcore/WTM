/**
 *  导出
 * /admin/role
 */

// 获取session
const getExportInfoService = {
    url: "/uds/ss/backend/v1/export",
    method: "post"
};
// 获取状态
const getProgressService = {
    url: "/uds/ss/backend/v1/export/progresses/{session}",
    method: "get"
};
// 获取下载地址
const getDownloadInfo = {
    url: "/uds/ss/backend/v1/export/download/{session}",
    method: "get"
};

export { getExportInfoService, getProgressService, getDownloadInfo };

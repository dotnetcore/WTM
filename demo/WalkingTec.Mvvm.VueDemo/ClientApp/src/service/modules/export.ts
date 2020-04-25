/**
 *  导出
 * /admin/role
 */

// 获取session
const getExportInfoService = {
  url: "/export",
  method: "post"
};
// 获取状态
const getProgressService = {
  url: "/export/progresses/{session}",
  method: "get"
};
// 获取下载地址
const getDownloadInfo = {
  url: "/export/download/{session}",
  method: "get"
};

export { getExportInfoService, getProgressService, getDownloadInfo };

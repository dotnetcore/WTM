using System;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// 文件相关扩展类，提供生成导出文件结果等方法
    /// </summary>
    public static class FileExtension
    {
        /// <summary>
        /// 生成下载文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="ExportName"></param>
        /// <returns></returns>
        public static FileContentResult GetExportData<T>(this IBasePagedListVM<T, BaseSearcher> self, string ExportName = "") where T : TopBasePoco, new()
        {
            // 已勾选数据行时按勾选导出，否则按搜索条件整体导出
            self.SearcherMode = self.Ids != null && self.Ids.Count > 0 ? ListVMSearchModeEnum.CheckExport : ListVMSearchModeEnum.Export;
            var data = self.GenerateExcel();
            // 分多个文件导出时打包为 zip，否则为单个 Excel 文件
            string ContentType = self.ExportExcelCount > 1 ? "application/x-zip-compresse" : "application/vnd.ms-excel";
            // 未指定导出文件名时默认使用实体类型名
            ExportName = string.IsNullOrEmpty(ExportName) ? typeof(T).Name : ExportName;
            // 生成带时间戳的下载文件名，扩展名随导出文件数量变化
            ExportName = self.ExportExcelCount > 1 ? $"Export_{ExportName}_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.zip" : $"Export_{ExportName}_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.xlsx";
            FileContentResult Result = new FileContentResult(data, ContentType);
            Result.FileDownloadName = ExportName;
            return Result;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
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
            self.SearcherMode = self.Ids != null && self.Ids.Count > 0 ? ListVMSearchModeEnum.CheckExport : ListVMSearchModeEnum.Export;
            var data = self.GenerateExcel();
            string ContentType = self.ExportExcelCount > 1 ? "application/x-zip-compresse" : "application/vnd.ms-excel";
            ExportName = string.IsNullOrEmpty(ExportName) ? typeof(T).Name : ExportName;
            ExportName = self.ExportExcelCount > 1 ? $"Export_{ExportName}_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.zip" : $"Export_{ExportName}_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}.xlsx";
            FileContentResult Result = new FileContentResult(data, ContentType);
            Result.FileDownloadName = ExportName;
            return Result;
        }

    }
}

using System;
using System.Linq.Expressions;

namespace WalkingTec.Mvvm.Core.Extensions
{
    public static class PagedListExtension
    {
        #region MakeGridColumn  生成GridColumn
        /// <summary>
        /// 生成GridColumn
        /// </summary>
        /// <typeparam name="T">继承自TopBasePoco的类</typeparam>
        /// <typeparam name="V">继承自ISearcher的类</typeparam>
        /// <param name="self">self</param>
        /// <param name="ColumnExp">指向T中字段的表达式</param>
        /// <param name="Format">格式化显示内容的委托函数，函数接受两个参数，第一个是整行数据，第二个是所选列的数据</param>
        /// <param name="Header">表头名称</param>
        /// <param name="Width">列宽</param>
        /// <param name="Flex">是否填充</param>
        /// <param name="AllowMultiLine">是否允许多行</param>
        /// <param name="NeedGroup">是否需要分组</param>
        /// <param name="ForeGroundFunc">设置前景色的委托函数</param>
        /// <param name="BackGroundFunc">设置背景色的委托函数</param>
        /// <returns>返回设置好的GridColumn类的实例</returns>
        public static GridColumn<T> MakeGridColumn<T, V>(this IBasePagedListVM<T, V> self
            , Expression<Func<T, object>> ColumnExp
            , ColumnFormatCallBack<T> Format = null
            , string Header = null
            , int? Width = null
            , int? Flex = null
            , bool AllowMultiLine = true
            , bool NeedGroup = false
            , Func<T, string> ForeGroundFunc = null
            , Func<T, string> BackGroundFunc = null)
            where T : TopBasePoco
            where V : ISearcher
        {
            GridColumn<T> rv = new GridColumn<T>(ColumnExp, Format, Header, Width, Flex, AllowMultiLine, NeedGroup, ForeGroundFunc, BackGroundFunc);
            return rv;
        }
        #endregion

        #region MakeActionGridColumn  生成Grid动作列(增删该查)
        ///// <summary>
        ///// 生成Grid动作列(增删该查)
        ///// </summary>
        ///// <typeparam name="T">继承自TopBasePoco的类</typeparam>
        ///// <typeparam name="V">继承自ISearcher的类</typeparam>
        ///// <param name="self">self</param>
        ///// <param name="Header">表头名称</param>
        ///// <param name="Width">列宽</param>
        ///// <returns>返回设置好的动作列</returns>
        //public static GridColumn<T> MakeActionGridColumn<T, V>(this IBasePagedListVM<T, V> self
        //    , string Header = null
        //    , int? Width = null)
        //    where T : TopBasePoco
        //    where V : ISearcher
        //{
        //    ActionGridColumn<T> rv = new ActionGridColumn<T>(Header, Width);
        //    rv.Sortable = false;
        //    return rv;
        //}
        #endregion

        #region MakeGridAction  创建一个新的列表动作
        ///// <summary>
        ///// 创建一个新的列表动作
        ///// </summary>
        ///// <typeparam name="T">继承自TopBasePoco的类</typeparam>
        ///// <typeparam name="V">继承自ISearcher的类</typeparam>
        ///// <param name="self">self</param>
        ///// <param name="name">动作名</param>
        ///// <param name="dialogTitle">弹出窗口标题</param>
        ///// <param name="controllerName">动作的Controller</param>
        ///// <param name="actionName">动作的Action</param>
        ///// <param name="paraType">动作类型</param>
        ///// <param name="showInRow">是否在每行都显示</param>
        ///// <param name="areaName">域名</param>
        ///// <param name="showDialog">是否需要弹出窗口，默认为true</param>
        ///// <param name="dialogWidth">弹出窗口的宽度</param>
        ///// <param name="dialogHeight">弹出窗口的高度</param>
        ///// <param name="iconCls">动作图标css，默认为null，没有图标</param>
        ///// <param name="hideOnToolBar">是否在工具栏隐藏  默认false</param>
        ///// <param name="buttonId">Button的id  默认自动生成</param>
        ///// <param name="isRedirect">是否在新页面打开</param>
        ///// <returns>列表动作</returns>
        //public static GridAction MakeGridAction<T, V>(this IBasePagedListVM<T, V> self
        //    , string name
        //    , string dialogTitle
        //    , string controllerName
        //    , string actionName
        //    , GridActionParameterTypesEnum paraType
        //    , bool showInRow
        //    , string areaName = null
        //    , bool showDialog = true
        //    , int? dialogWidth = null
        //    , int? dialogHeight = null
        //    , string iconCls = null
        //    , bool hideOnToolBar = false
        //    , string buttonId = null
        //    , bool isRedirect = false
        //    , bool resizable = true)
        //    where T : TopBasePoco
        //    where V : ISearcher
        //{
        //    return new GridAction { Name = name, Area = areaName, ControllerName = controllerName, DialogTitle = dialogTitle, ActionName = actionName, DialogWidth = dialogWidth, DialogHeight = dialogHeight, ParameterType = paraType, ShowInRow = showInRow, OnClickFunc = null, ShowDialog = showDialog, IconCls = iconCls, HideOnToolBar = hideOnToolBar, ButtonId = buttonId, IsRedirect = isRedirect, Resizable = resizable };
        //}
        #endregion

        #region MakeScriptGridAction  创建一个执行自定义js的列表动作
        ///// <summary>
        ///// 创建一个执行自定义js的列表动作
        ///// </summary>
        ///// <typeparam name="T">继承自TopBasePoco的类</typeparam>
        ///// <typeparam name="V">继承自ISearcher的类</typeparam>
        ///// <param name="self">self</param>
        ///// <param name="controllerName">动作的Controller</param>
        ///// <param name="actionName">动作的Action</param>
        ///// <param name="name">动作名</param>
        ///// <param name="script">点击动作按钮执行的js</param>
        ///// <param name="showInRow">是否在操作列中显示  默认true</param>
        ///// <param name="hideOnToolBar">是否在工具栏中隐藏  默认true</param>
        ///// <param name="areaName">动作的区域</param>
        ///// <param name="buttonId">Button的id  默认自动生成</param>
        ///// <returns>列表动作</returns>
        ///// <remarks>
        ///// 框架的自动权限验证无法验证自定义的js中跳转到的链接，
        ///// 所以虽然只是运行js，但如果js最终跳转到某个链接，则还是需要在这里指定Controller和Action，这样框架就可以自动判断权限
        ///// </remarks>
        //public static GridAction MakeScriptGridAction<T, V>(this IBasePagedListVM<T, V> self
        //    , string controllerName
        //    , string actionName
        //    , string name
        //    , string script
        //    , string areaName = null
        //    , string buttonId = null
        //    , bool showInRow = true
        //    , bool hideOnToolBar = true
        //    , bool resizable = true)//add by wuwh 2014.05.07  添加hideOnToolBar参数
        //    where T : TopBasePoco
        //    where V : ISearcher
        //{
        //    return new GridAction { Area = areaName, ControllerName = controllerName, ActionName = actionName, Name = name, OnClickFunc = script, ShowInRow = showInRow, ButtonId = buttonId, HideOnToolBar = hideOnToolBar, Resizable = resizable };
        //}
        #endregion

        #region MakeStandardAction  创建标准动作
        ////新增参数 whereStr
        ///// <summary>
        ///// 创建标准动作
        ///// </summary>
        ///// <typeparam name="T">继承自TopBasePoco的类</typeparam>
        ///// <typeparam name="V">继承自ISearcher的类</typeparam>
        ///// <param name="self">self</param>
        ///// <param name="controllerName">动作的Controller</param>
        ///// <param name="standardType">标准动作类型</param>
        ///// <param name="dialogTitle">弹出窗口的标题</param>
        ///// <param name="areaName">域名</param>
        ///// <param name="dialogWidth">弹出窗口的宽度</param>
        ///// <param name="dialogHeight">弹出窗口的高度</param>
        ///// <param name="name">动作名，默认为‘新建’</param>
        ///// <param name="buttonId">Button的id  默认自动生成</param>
        ///// <returns>列表动作</returns>
        ///// <remarks>
        ///// 根据标准动作类型，创建默认属性的标准动作
        ///// </remarks>
        //public static GridAction MakeStandardAction<T, V>(this IBasePagedListVM<T, V> self
        //    , string controllerName
        //    , GridActionStandardTypesEnum standardType
        //    , string dialogTitle
        //    , string areaName = null
        //    , int? dialogWidth = null
        //    , int? dialogHeight = null
        //    , string name = null
        //    , string buttonId = null
        //    , bool resizable = true
        //    , params Expression<Func<T, object>>[] whereStr)
        //    where T : TopBasePoco
        //    where V : ISearcher
        //{
        //    string iconcls = "";
        //    string action = "";
        //    string gridname = "";
        //    GridActionParameterTypesEnum paraType = GridActionParameterTypesEnum.NoId;
        //    bool showinrow = false;
        //    bool hideontoolbar = false;
        //    switch (standardType)
        //    {
        //        case GridActionStandardTypesEnum.Create:
        //            iconcls = "icon-add";
        //            action = "Create";
        //            gridname = "新建";
        //            paraType = GridActionParameterTypesEnum.NoId;
        //            break;
        //        case GridActionStandardTypesEnum.Edit:
        //            iconcls = "icon-edit";
        //            action = "Edit";
        //            gridname = "修改";
        //            paraType = GridActionParameterTypesEnum.SingleId;
        //            showinrow = true;
        //            hideontoolbar = true;
        //            break;
        //        case GridActionStandardTypesEnum.Delete:
        //            iconcls = "icon-delete";
        //            action = "Delete";
        //            gridname = "删除";
        //            paraType = GridActionParameterTypesEnum.SingleId;
        //            showinrow = true;
        //            hideontoolbar = true;
        //            break;
        //        case GridActionStandardTypesEnum.Details:
        //            iconcls = "icon-details";
        //            action = "Details";
        //            gridname = "详细";
        //            paraType = GridActionParameterTypesEnum.SingleId;
        //            showinrow = true;
        //            hideontoolbar = true;
        //            break;
        //        case GridActionStandardTypesEnum.BatchEdit:
        //            iconcls = "icon-edit";
        //            action = "BatchEdit";
        //            gridname = "批量修改";
        //            paraType = GridActionParameterTypesEnum.MultiIds;
        //            break;
        //        case GridActionStandardTypesEnum.BatchDelete:
        //            iconcls = "icon-delete";
        //            action = "BatchDelete";
        //            gridname = "批量删除";
        //            paraType = GridActionParameterTypesEnum.MultiIds;
        //            break;
        //        case GridActionStandardTypesEnum.Import:
        //            iconcls = "icon-details";
        //            action = "ImportExcelData";
        //            gridname = "导入";
        //            paraType = GridActionParameterTypesEnum.NoId;
        //            break;
        //        default:
        //            break;
        //    }
        //    List<string> list = new List<string>();
        //    foreach (var item in whereStr)
        //    {
        //        list.Add(PropertyHelper.GetPropertyName(item));
        //    }

        //    return new GridAction { Name = (name == null ? gridname : name), DialogTitle = dialogTitle, IconCls = iconcls, ControllerName = controllerName, ActionName = action, Area = areaName, DialogWidth = dialogWidth, DialogHeight = dialogHeight, ParameterType = paraType, ShowInRow = showinrow, ShowDialog = true, HideOnToolBar = hideontoolbar, ButtonId = buttonId, Resizable = resizable, whereStr = list.ToArray() };
        //}
        #endregion

        #region MakeStandardExportAction  创建标准导出按钮
        ///// <summary>
        ///// 创建标准导出按钮
        ///// </summary>
        ///// <typeparam name="T">继承自TopBasePoco的类</typeparam>
        ///// <typeparam name="V">继承自ISearcher的类</typeparam>
        ///// <param name="self">self</param>
        ///// <param name="gridid">vmGuid</param>
        ///// <param name="MustSelect"></param>
        ///// <param name="exportType">导出类型  默认null，支持所有导出</param>
        ///// <param name="param">参数</param>
        ///// <returns></returns>
        //public static GridAction MakeStandardExportAction<T, V>(this IBasePagedListVM<T, V> self
        //    , string gridid = null
        //    , bool MustSelect = false
        //    , ExportEnum? exportType = null
        //    , params KeyValuePair<string, string>[] param)
        //    where T : TopBasePoco
        //    where V : ISearcher
        //{
        //    string excelscript = "";
        //    string pdfscript = "";

        //    string parameters = "?";
        //    foreach (var item in param)
        //    {
        //        parameters += item.Key + "=" + item.Value + "&";
        //    }
        //    if (self.FromFixedCon == true)
        //    {
        //        parameters += "DONOTUSECSName=" + self.CurrentCS + "&";
        //    }
        //    parameters += "1=1";
        //    if (gridid == null)
        //    {
        //        gridid = "Grid" + self.UniqueId;
        //    }
        //    if (MustSelect == false)
        //    {
        //        excelscript = string.Format("FF_ShowMask(\"{1}\");FF_DownloadExcelOrPdfPost(\"{0}\",\"/WebApi/Home/ExportExcel{2}\");", gridid, "正在生成文件...", parameters);
        //        pdfscript = string.Format("FF_ShowMask(\"{1}\");FF_DownloadExcelOrPdfPost(\"{0}\",\"/WebApi/Home/ExportPdf{2}\");", gridid, "正在生成文件...", parameters);
        //    }
        //    else
        //    {
        //        excelscript += "var sels = Ext.getCmp(\"" + gridid + "\").getSelectionModel().getSelection();";
        //        excelscript += "if(sels.length == 0){ FF_OpenSimpleDialog(\"" + "错误" + "\",\"" + "请至少选择一条数据" + "\");} else{";
        //        excelscript += string.Format("FF_ShowMask(\"{1}\");FF_DownloadExcelOrPdfPost(\"{0}\",\"/WebApi/Home/ExportExcel{2}\");", gridid, "正在生成文件...", parameters) + "}";

        //        pdfscript += "var sels = Ext.getCmp(\"" + gridid + "\").getSelectionModel().getSelection();";
        //        pdfscript += "if(sels.length == 0){ FF_OpenSimpleDialog(\"" + "错误" + "\",\"" + "请至少选择一条数据" + "\");} else{";
        //        pdfscript += string.Format("FF_ShowMask(\"{1}\");FF_DownloadExcelOrPdfPost(\"{0}\",\"/WebApi/Home/ExportPdf{2}\");", gridid, "正在生成文件...", parameters) + "}";
        //    }
        //    GridAction ga = null;
        //    if (exportType == ExportEnum.Excel)
        //    {
        //        ga = new GridAction { Area = "WebApi", ControllerName = "Home", ActionName = "ExportExcel", Name = "导出Excel", ParameterType = GridActionParameterTypesEnum.NoId, OnClickFunc = excelscript, ShowInRow = false, Resizable = true };
        //    }
        //    else if (exportType == ExportEnum.PDF)
        //    {
        //        ga = new GridAction { Area = "WebApi", ControllerName = "Home", ActionName = "ExportExcel", Name = "导出PDF", ParameterType = GridActionParameterTypesEnum.NoId, OnClickFunc = excelscript, ShowInRow = false, Resizable = true };
        //    }
        //    else
        //    {
        //        ga = new GridAction { Area = "WebApi", ControllerName = "Home", ActionName = "ExportExcel", Name = "导出", ParameterType = GridActionParameterTypesEnum.NoId, OnClickFunc = excelscript, ShowInRow = false, Resizable = true };
        //        GridAction excel = new GridAction { Area = "WebApi", ControllerName = "Home", ActionName = "ExportExcel", Name = "导出Excel", ParameterType = GridActionParameterTypesEnum.NoId, OnClickFunc = excelscript, ShowInRow = false, Resizable = true };
        //        GridAction pdf = new GridAction { Area = "WebApi", ControllerName = "Home", ActionName = "ExportPdf", Name = "导出PDF", ParameterType = GridActionParameterTypesEnum.NoId, OnClickFunc = pdfscript, ShowInRow = false, Resizable = true };
        //        ga.SubActions = new List<GridAction> { excel, pdf };
        //    }
        //    return ga;
        //}
        #endregion

    }
}
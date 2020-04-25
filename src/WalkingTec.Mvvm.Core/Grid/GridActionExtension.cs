using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// GridActionExtension
    /// </summary>
    public static class GridActionExtensions
    {
        #region MakeStandardAction  创建标准动作

        /// <summary>
        /// 创建标准动作
        /// </summary>
        /// <typeparam name="T">继承自TopBasePoco的类</typeparam>
        /// <typeparam name="V">继承自ISearcher的类</typeparam>
        /// <param name="self">self</param>
        /// <param name="controllerName">动作的Controller</param>
        /// <param name="standardType">标准动作类型</param>
        /// <param name="dialogTitle">弹出窗口的标题，可为空，代表使用默认文字</param>
        /// <param name="areaName">域名</param>
        /// <param name="dialogWidth">弹出窗口的宽度</param>
        /// <param name="dialogHeight">弹出窗口的高度</param>
        /// <param name="name">按钮显示的文字</param>
        /// <param name="buttonId">Button的id  默认自动生成</param>
        /// <param name="whereStr">whereStr</param>
        /// <returns>列表动作</returns>
        /// <remarks>
        /// 根据标准动作类型，创建默认属性的标准动作
        /// </remarks>
        public static GridAction MakeStandardAction<T, V>(this IBasePagedListVM<T, V> self
            , string controllerName
            , GridActionStandardTypesEnum standardType
            , string dialogTitle
            , string areaName = null
            , int? dialogWidth = null
            , int? dialogHeight = null
            , string name = null
            , string buttonId = null
            , params Expression<Func<T, object>>[] whereStr)
            where T : TopBasePoco
            where V : ISearcher
        {
            var iconcls = string.Empty;
            var actionName = standardType.ToString();
            var gridname = string.Empty;
            var paraType = GridActionParameterTypesEnum.NoId;
            var showInRow = false;
            var hideOnToolBar = false;
            var showDialog = true;

            switch (standardType)
            {
                case GridActionStandardTypesEnum.Create:
                    iconcls = "layui-icon layui-icon-add-1";
                    gridname = Program._localizer["Create"];
                    paraType = GridActionParameterTypesEnum.NoId;
                    break;
                case GridActionStandardTypesEnum.AddRow:
                    iconcls = "layui-icon layui-icon-add-1";
                    gridname = Program._localizer["Create"];
                    paraType = GridActionParameterTypesEnum.AddRow;
                    break;
                case GridActionStandardTypesEnum.Edit:
                    iconcls = "layui-icon layui-icon-edit";
                    gridname = Program._localizer["Edit"];
                    paraType = GridActionParameterTypesEnum.SingleId;
                    showInRow = true;
                    hideOnToolBar = true;
                    break;
                case GridActionStandardTypesEnum.Delete:
                    iconcls = "layui-icon layui-icon-delete";
                    gridname = Program._localizer["Delete"];
                    paraType = GridActionParameterTypesEnum.SingleId;
                    showInRow = true;
                    hideOnToolBar = true;
                    break;
                case GridActionStandardTypesEnum.RemoveRow:
                    iconcls = "layui-icon layui-icon-delete";
                    gridname = Program._localizer["Delete"];
                    paraType = GridActionParameterTypesEnum.RemoveRow;
                    showInRow = true;
                    hideOnToolBar = true;
                    break;
                case GridActionStandardTypesEnum.Details:
                    iconcls = "layui-icon layui-icon-form";
                    gridname = Program._localizer["Details"];
                    paraType = GridActionParameterTypesEnum.SingleId;
                    showInRow = true;
                    hideOnToolBar = true;
                    break;
                case GridActionStandardTypesEnum.BatchEdit:
                    iconcls = "layui-icon layui-icon-edit";
                    gridname = Program._localizer["BatchEdit"];
                    paraType = GridActionParameterTypesEnum.MultiIds;
                    break;
                case GridActionStandardTypesEnum.BatchDelete:
                    iconcls = "layui-icon layui-icon-delete";
                    gridname = Program._localizer["BatchDelete"];
                    paraType = GridActionParameterTypesEnum.MultiIds;
                    break;
                case GridActionStandardTypesEnum.Import:
                    iconcls = "layui-icon layui-icon-templeate-1";
                    gridname = Program._localizer["Import"];
                    paraType = GridActionParameterTypesEnum.NoId;
                    break;
                case GridActionStandardTypesEnum.ExportExcel:
                    iconcls = "layui-icon layui-icon-download-circle";
                    gridname = Program._localizer["Export"];
                    paraType = GridActionParameterTypesEnum.MultiIdWithNull;
                    name = Program._localizer["ExportExcel"];
                    showInRow = false;
                    showDialog = false;
                    hideOnToolBar = false;
                   break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(dialogTitle))
            {
                dialogTitle = gridname;
            }

            var list = new List<string>();
            foreach (var item in whereStr)
            {
                list.Add(PropertyHelper.GetPropertyName(item));
            }

            return new GridAction
            {
                ButtonId = buttonId,
                Name = (name ?? gridname),
                DialogTitle = dialogTitle,
                Area = areaName,
                ControllerName = controllerName,
                ActionName = actionName,
                ParameterType = paraType,

                IconCls = iconcls,
                DialogWidth = dialogWidth ?? 800,
                DialogHeight = dialogHeight,
                ShowInRow = showInRow,
                ShowDialog = showDialog,
                HideOnToolBar = hideOnToolBar,
                whereStr = list.ToArray()
            };
        }

        #endregion

        #region MakeAction 创建按钮
        /// <summary>
        /// 创建标准动作
        /// </summary>
        /// <typeparam name="T">继承自TopBasePoco的类</typeparam>
        /// <typeparam name="V">继承自ISearcher的类</typeparam>
        /// <param name="self">self</param>
        /// <param name="controllerName">动作的Controller</param>
        /// <param name="actionName">actionName</param>
        /// <param name="name">动作名，默认为‘新建’</param>
        /// <param name="dialogTitle">弹出窗口的标题</param>
        /// <param name="paraType">paraType</param>
        /// <param name="areaName">域名</param>
        /// <param name="dialogWidth">弹出窗口的宽度</param>
        /// <param name="dialogHeight">弹出窗口的高度</param>
        /// <param name="buttonId">Button的id  默认自动生成</param>
        /// <param name="whereStr">whereStr</param>
        /// <returns>列表动作</returns>
        /// <remarks>
        /// 根据标准动作类型，创建默认属性的标准动作
        /// </remarks>
        public static GridAction MakeAction<T, V>(this IBasePagedListVM<T, V> self
            , string controllerName
            , string actionName
            , string name
            , string dialogTitle
            , GridActionParameterTypesEnum paraType
            , string areaName = null
            , int? dialogWidth = null
            , int? dialogHeight = null
            , string buttonId = null
            , params Expression<Func<T, object>>[] whereStr)
            where T : TopBasePoco
            where V : ISearcher
        {
            var iconcls = string.Empty;

            var list = new List<string>();
            foreach (var item in whereStr)
            {
                list.Add(PropertyHelper.GetPropertyName(item));
            }

            return new GridAction
            {
                ButtonId = buttonId,
                Name = name,
                DialogTitle = dialogTitle,
                Area = areaName,
                ControllerName = controllerName,
                ActionName = actionName,
                ParameterType = paraType, 
                IconCls = iconcls,
                DialogWidth = dialogWidth ?? 800,
                DialogHeight = dialogHeight,
                ShowDialog = true,
                whereStr = list.ToArray()
            };
        }

        public static GridAction MakeActionsGroup<T, V>(this IBasePagedListVM<T, V> self
            , string name
            , List<GridAction> subActions
            , params Expression<Func<T, object>>[] whereStr)
            where T : TopBasePoco
            where V : ISearcher
        {
            var iconcls = string.Empty;

            var list = new List<string>();
            foreach (var item in whereStr)
            {
                list.Add(PropertyHelper.GetPropertyName(item));
            }

            return new GridAction
            {
                ButtonId = Guid.NewGuid().ToString(),
                Name = name,
                DialogTitle = "",
                Area = "",
                ControllerName = "",
                ActionName = "ActionsGroup",
                ParameterType =  GridActionParameterTypesEnum.NoId, 
                IconCls = iconcls,
                DialogWidth = 0,
                DialogHeight = 0,
                ShowDialog = false,
                whereStr = list.ToArray(),
                 SubActions= subActions
            };
        }

        #endregion

        #region MakeStandardExportAction  创建标准导出按钮

        /// <summary>
        /// 创建标准导出按钮
        /// </summary>
        /// <typeparam name="T">继承自TopBasePoco的类</typeparam>
        /// <typeparam name="V">继承自ISearcher的类</typeparam>
        /// <param name="self">self</param>
        /// <param name="gridid">vmGuid</param>
        /// <param name="MustSelect"></param>
        /// <param name="exportType">导出类型  默认null，支持所有导出</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        [Obsolete("Will be removed in future, use MakeStandardAction with GridActionStandardTypesEnum.ExportExcel instead")]
        public static GridAction MakeStandardExportAction<T, V>(this IBasePagedListVM<T, V> self
            , string gridid = null
            , bool MustSelect = false
            , ExportEnum? exportType = null
            , params KeyValuePair<string, string>[] param)
            where T : TopBasePoco
            where V : ISearcher
        {
            exportType = ExportEnum.Excel;

            var action = new GridAction
            {
                Name = Program._localizer["ExportExcel"],
                DialogTitle = Program._localizer["ExportExcel"],
                Area = string.Empty,
                ControllerName = "_Framework",
                ActionName = "GetExportExcel",
                ParameterType = GridActionParameterTypesEnum.MultiIdWithNull,

                IconCls = "layui-icon layui-icon-download-circle",
                ShowInRow = false,
                ShowDialog = false,
                HideOnToolBar = false
            };
            return action;
        }

        #endregion

        #region Set Property

        /// <summary>
        /// Set the dialog to be maximized
        /// </summary>
        /// <param name="self"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        public static GridAction SetMax(this GridAction self, bool Max = true)
        {
            self.Max = Max;
            return self;
        }


        /// <summary>
        /// Set the dialog to be maximized
        /// </summary>
        /// <param name="self"></param>
        /// <param name="buttonclass">button class.
        /// Some of the layui defined class to control color:
        /// layui-btn-primary
        /// layui-btn-normal
        /// layui-btn-warm
        /// layui-btn-danger
        /// </param>
        /// <returns></returns>
        public static GridAction SetButtonClass(this GridAction self, string buttonclass)
        {
            self.ButtonClass = buttonclass;
            return self;
        }



        /// <summary>
        /// Set the dialog to be maximized
        /// </summary>
        /// <param name="self"></param>
        /// <param name="isDownload"></param>
        /// <returns></returns>
        public static GridAction SetIsDownload(this GridAction self, bool isDownload = true)
        {
            self.IsDownload = isDownload;
            return self;
        }

        /// <summary>
        /// Set prompt message
        /// </summary>
        /// <param name="self"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static GridAction SetPromptMessage(this GridAction self, string msg)
        {
            self.PromptMessage = msg;
            return self;
        }


        /// <summary>
        /// 是否在每行都显示
        /// </summary>
        /// <param name="self"></param>
        /// <param name="showInRow"></param>
        /// <returns></returns>
        public static GridAction SetShowInRow(this GridAction self, bool showInRow = true)
        {
            self.ShowInRow = showInRow;
            return self;
        }
        /// <summary>
        /// 是否在工具栏上隐藏按钮
        /// </summary>
        /// <param name="self"></param>
        /// <param name="hideOnToolBar"></param>
        /// <returns></returns>
        public static GridAction SetHideOnToolBar(this GridAction self, bool hideOnToolBar = true)
        {
            self.HideOnToolBar = hideOnToolBar;
            return self;
        }
        /// <summary>
        /// 把按钮当作容器,添加按钮的子按钮
        /// </summary>
        /// <param name="self"></param>
        /// <param name="subActions">子按钮</param>
        /// <returns></returns>
        public static GridAction SetSubActions(this GridAction self, List<GridAction> subActions)
        {
            self.SubActions = subActions;
            return self;
        }
        #endregion
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace WalkingTec.Mvvm.Core
{
    #region 列表动作类型
    /// <summary>
    /// 列表动作类型
    /// </summary>
    public enum GridActionParameterTypesEnum
    {
        /// <summary>
        /// 不需要传递Id
        /// </summary>
        NoId,
        /// <summary>
        /// 只传递一个Id
        /// </summary>
        SingleId,
        /// <summary>
        /// 传递多个Id
        /// </summary>
        MultiIds,
        /// <summary>
        /// 只传递一个Id，但 Id 可能为null
        /// </summary>
        SingleIdWithNull,
        /// <summary>
        /// 传递多个 Id 或 null
        /// </summary>
        MultiIdWithNull,
        AddRow,
        RemoveRow
    }

    #endregion


    #region 标准列表动作
    /// <summary>
    /// 标准列表动作
    /// </summary>
    public enum GridActionStandardTypesEnum
    {
        Create,
        Edit,
        Delete,
        Details,
        BatchEdit,
        BatchDelete,
        Import,
        ExportExcel,
        AddRow,
        RemoveRow,
        ActionsGroup
    }
    #endregion

    #region 导出枚举
    public enum ExportEnum
    {
        [Display(Name = "Excel")]
        Excel = 0,
        [Display(Name = "PDF")]
        PDF = 1
    }
    #endregion

    /// <summary>
    /// 列表动作类，负责处理列表动作条中的动作按钮
    /// </summary>
    public class GridAction
    {
        #region Action属性

        /// <summary>
        /// 按钮Id，一般不需要设定，系统会自动生成唯一Id。如果设定请确保 Id 的唯一性
        /// </summary>
        public string ButtonId { get; set; }

        /// <summary>
        /// 按钮名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 弹出窗口的标题
        /// </summary>
        public string DialogTitle { get; set; }

        /// <summary>
        /// 如果不为null，则只运行这个变量设定的script，其他的属性都不起作用
        /// </summary>
        public string OnClickFunc { get; set; }

        /// <summary>
        /// 是否在每行都显示
        /// </summary>
        public bool ShowInRow { get; set; }
        /// <summary>
        /// 是否在工具栏上隐藏按钮
        /// </summary>
        public bool HideOnToolBar { get; set; }

        /// <summary>
        /// bind to a column name to deside whether or not to show this action
        /// </summary>
        public string BindVisiableColName { get; set; }

        /// <summary>
        /// additional css class of button
        /// </summary>
        public string ButtonClass { get; set;}
        /// <summary>
        /// if the dialog need to be maximax
        /// </summary>
        public bool Max { get; set; }

        /// <summary>
        /// If this action is to download a file
        /// </summary>
        public bool IsDownload { get; set; }

        #region 请求链接相关

        /// <summary>
        /// 动作的Area
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 动作的Controller
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// 动作的Action
        /// </summary>
        public string ActionName { get; set; }

        public string Url
        {
            get
            {
                var rv = "";
                if(string.IsNullOrEmpty(ControllerName) == false){
                    rv = $"/{HttpUtility.UrlEncode(ControllerName)}/{HttpUtility.UrlEncode(ActionName)}";
                    if (!string.IsNullOrEmpty(Area))
                    {
                        rv = $"/{HttpUtility.UrlEncode(Area)}{rv}";
                    }
                    if (!string.IsNullOrEmpty(QueryString))
                    {
                        rv = $"{rv}?{QueryString}";
                    }
                    else
                    {
                        rv = $"{rv}?1=1"; ;
                    }
                }
                return rv;
            }
        }

        #endregion

        /// <summary>
        /// 是否跳转到新页面
        /// </summary>
        public bool IsRedirect { get; set; }

        /// <summary>
        /// 弹出问询框
        /// </summary>
        public string PromptMessage { get; set; }

        /// <summary>
        /// 动作类型
        /// </summary>
        public GridActionParameterTypesEnum ParameterType { get; set; }

        #endregion

        #region 暂时无用
        /// <summary>
        /// 是否可以resizable
        /// </summary>
        public bool Resizable { get; set; }
        /// <summary>
        /// 动作图标css
        /// </summary>
        public string IconCls { get; set; }
        /// <summary>
        /// 动作的QueryString
        /// </summary>
        public string QueryString { get; set; }
        /// <summary>
        /// 弹出窗口的宽度
        /// </summary>
        public int? DialogWidth { get; set; }
        /// <summary>
        /// 弹出窗口的高度
        /// </summary>
        public int? DialogHeight { get; set; }
        /// <summary>
        /// 是否需要弹出窗口
        /// </summary>
        public bool ShowDialog { get; set; }

        /// <summary>
        /// 如果设定了SubActions，则代表需要用SplitButton的形式展示，主GridAction将不起作用
        /// </summary>
        public List<GridAction> SubActions { get; set; }

        public string[] whereStr { get; set; }

        #endregion
    }
}

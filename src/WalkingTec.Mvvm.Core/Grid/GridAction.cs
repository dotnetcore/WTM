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
        /// <summary>
        /// 子表格新增行，不需要传递Id
        /// </summary>
        AddRow,
        /// <summary>
        /// 子表格删除行，传递要删除行的Id
        /// </summary>
        RemoveRow
    }

    #endregion


    #region 标准列表动作
    /// <summary>
    /// 标准列表动作
    /// </summary>
    public enum GridActionStandardTypesEnum
    {
        /// <summary>
        /// 新建动作，不需要传递Id
        /// </summary>
        Create,
        /// <summary>
        /// 编辑动作，传递单个Id
        /// </summary>
        Edit,
        /// <summary>
        /// 删除动作，传递单个Id
        /// </summary>
        Delete,
        /// <summary>
        /// 简单删除动作，传递单个Id，不弹出窗口直接提交删除
        /// </summary>
        SimpleDelete,
        /// <summary>
        /// 查看详情动作，传递单个Id
        /// </summary>
        Details,
        /// <summary>
        /// 批量修改动作，传递多个Id
        /// </summary>
        BatchEdit,
        /// <summary>
        /// 批量删除动作，传递多个Id
        /// </summary>
        BatchDelete,
        /// <summary>
        /// 简单批量删除动作，传递多个Id，不弹出窗口直接提交删除
        /// </summary>
        SimpleBatchDelete,
        /// <summary>
        /// 导入动作，不需要传递Id
        /// </summary>
        Import,
        /// <summary>
        /// 导出Excel动作，可传递多个Id或null
        /// </summary>
        ExportExcel,
        /// <summary>
        /// 子表格新增行动作
        /// </summary>
        AddRow,
        /// <summary>
        /// 子表格删除行动作
        /// </summary>
        RemoveRow,
        /// <summary>
        /// 动作组，将多个子动作合并为一个下拉按钮组展示
        /// </summary>
        ActionsGroup,
        /// <summary>
        /// 审批动作，传递单个Id
        /// </summary>
        Approve
    }
    #endregion

    #region 导出枚举
    /// <summary>
    /// 导出格式枚举
    /// </summary>
    public enum ExportEnum
    {
        /// <summary>
        /// 导出为Excel格式
        /// </summary>
        [Display(Name = "Excel")]
        Excel = 0,
        /// <summary>
        /// 导出为PDF格式
        /// </summary>
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
        /// 是否为导出动作
        /// </summary>
        public bool IsExport { get; set; }
        /// <summary>
        /// 动作的Controller
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// 动作的Action
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// 根据Area、Controller、Action和QueryString拼装出的请求地址
        /// </summary>
        public string Url
        {
            get
            {
                var rv = "";
                //设定了Controller时，拼装 /Area/Controller/Action 形式的地址，各段均做Url编码
                if(string.IsNullOrEmpty(ControllerName) == false){
                    rv = $"/{HttpUtility.UrlEncode(ControllerName)}/{HttpUtility.UrlEncode(ActionName)}";
                    if (!string.IsNullOrEmpty(Area))
                    {
                        rv = $"/{HttpUtility.UrlEncode(Area)}{rv}";
                    }
                    //有QueryString时附加查询参数，没有时附加“1=1”占位参数保证地址格式统一
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

        /// <summary>
        /// 是否强制使用Post方式提交请求
        /// </summary>
        public bool ForcePost { get; set; }
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

        /// <summary>
        /// 动作关联的字段名数组，用于在传递Id的同时把指定字段的值一并带入Url
        /// </summary>
        public string[] whereStr { get; set; }

        #endregion
    }
}

using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// GridActionExtension
    /// </summary>
    public static class GridActionExtension
    {
        /// <summary>
        /// 按钮Id，一般不需要设定，系统会自动生成唯一Id。如果设定请确保 Id 的唯一性
        /// </summary>
        /// <param name="self"></param>
        /// <param name="buttonId"></param>
        /// <returns></returns>
        public static GridAction SetButtonId(this GridAction self, string buttonId)
        {
            self.ButtonId = buttonId;
            return self;
        }
        /// <summary>
        /// 按钮名称
        /// </summary>
        /// <param name="self"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GridAction SetName(this GridAction self, string name)
        {
            self.Name = name;
            return self;
        }
        /// <summary>
        /// 弹出窗口的标题
        /// </summary>
        /// <param name="self"></param>
        /// <param name="dialogTitle"></param>
        /// <returns></returns>
        public static GridAction SetDialogTitle(this GridAction self, string dialogTitle)
        {
            self.DialogTitle = dialogTitle;
            return self;
        }
        /// <summary>
        /// 动作图标css
        /// </summary>
        /// <param name="self"></param>
        /// <param name="iconCls"></param>
        /// <returns></returns>
        public static GridAction SetIconCls(this GridAction self, string iconCls)
        {
            self.IconCls = iconCls;
            return self;
        }
        /// <summary>
        /// 动作的Area
        /// </summary>
        /// <param name="self"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public static GridAction SetArea(this GridAction self, string area)
        {
            self.Area = area;
            return self;
        }
        /// <summary>
        /// 动作的Controller
        /// </summary>
        /// <param name="self"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public static GridAction SetControllerName(this GridAction self, string controllerName)
        {
            self.ControllerName = controllerName;
            return self;
        }
        /// <summary>
        /// 动作的Action
        /// </summary>
        /// <param name="self"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public static GridAction SetActionName(this GridAction self, string actionName)
        {
            self.ActionName = actionName;
            return self;
        }
        /// <summary>
        /// 动作的QueryString
        /// </summary>
        /// <param name="self"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public static GridAction SetQueryString(this GridAction self, string queryString)
        {
            self.QueryString = queryString;
            return self;
        }
        /// <summary>
        /// 弹出窗口的宽度、高度
        /// </summary>
        /// <param name="self"></param>
        /// <param name="dialogWidth"></param>
        /// <param name="dialogHeight"></param>
        /// <returns></returns>
        public static GridAction SetSize(this GridAction self, int? dialogWidth, int? dialogHeight)
        {
            self.DialogWidth = dialogWidth;
            self.DialogHeight = dialogHeight;
            return self;
        }
        /// <summary>
        /// 是否需要弹出窗口
        /// </summary>
        /// <param name="self"></param>
        /// <param name="showDialog"></param>
        /// <returns></returns>
        public static GridAction SetShowDialog(this GridAction self, bool showDialog = true)
        {
            self.ShowDialog = showDialog;
            return self;
        }
        /// <summary>
        /// 是否跳转到新页面
        /// </summary>
        /// <param name="self"></param>
        /// <param name="isRedirect"></param>
        /// <returns></returns>
        public static GridAction SetIsRedirect(this GridAction self, bool isRedirect = true)
        {
            self.IsRedirect = isRedirect;
            return self;
        }
        /// <summary>
        /// 动作类型
        /// </summary>
        /// <param name="self"></param>
        /// <param name="parameterType"></param>
        /// <returns></returns>
        public static GridAction SetParameterType(this GridAction self, GridActionParameterTypesEnum parameterType)
        {
            self.ParameterType = parameterType;
            return self;
        }
        /// <summary>
        /// 如果不为null，则只运行这个变量设定的script，其他的属性都不起作用    
        /// </summary>
        /// <param name="self"></param>
        /// <param name="onClickScript"></param>
        /// <remarks>
        /// 如设置SetOnClickScript("test")，点击按钮时框架会调用页面上的javascript方法: function test(ids,datas){}
        /// ids是勾选的id数组，datas是勾选的所有字段数组
        /// </remarks>
        /// <returns></returns>
        public static GridAction SetOnClickScript(this GridAction self, string onClickScript)
        {
            self.OnClickFunc = onClickScript;
            return self;
        }
        /// <summary>
        /// 如果设定了SubActions，则代表需要用SplitButton的形式展示，主GridAction将不起作用
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gridActions"></param>
        /// <returns></returns>
        public static GridAction SetSubAction(this GridAction self, params GridAction[] gridActions)
        {
            if (self.SubActions == null)
            {
                self.SubActions = new List<GridAction>();
            }
            self.SubActions.AddRange(gridActions);
            return self;
        }
        /// <summary>
        /// 是否可以拖动改变弹出窗体大小
        /// </summary>
        /// <param name="self"></param>
        /// <param name="resizable"></param>
        /// <returns></returns>
        public static GridAction SetNotResizable(this GridAction self, bool resizable = false)
        {
            self.Resizable = resizable;
            return self;
        }

        /// <summary>
        /// 设置一个布尔值的列名，当改列值为true的时候才显示本行的这个动作按钮
        /// </summary>
        /// <param name="self"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static GridAction SetBindVisiableColName(this GridAction self, string colName)
        {
            self.BindVisiableColName = colName;
            return self;
        }
    }
}

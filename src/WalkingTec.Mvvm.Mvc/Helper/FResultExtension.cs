using System;

namespace WalkingTec.Mvvm.Mvc
{
    /// <summary>
    /// FResult 扩展类，提供链式拼接常用 LayUI 前端脚本的方法
    /// </summary>
    public static class FResultExtension
    {
        /// <summary>
        /// 创建一个关闭指定窗口的Result
        /// </summary>
        /// <param name="self">ContentResult</param>
        /// <returns>返回当前对象</returns>
        public static FResult CloseDialog(this FResult self)
        {
            self.ContentBuilder.Append("ff.CloseDialog();");
            return self;
        }

        /// <summary>
        /// 添加弹出警告框脚本
        /// </summary>
        /// <param name="self">FResult 实例</param>
        /// <param name="msg">提示消息内容</param>
        /// <param name="title">警告框标题，为空时使用本地化的默认标题</param>
        /// <returns>返回当前 FResult 对象，支持链式调用</returns>
        public static FResult Alert(this FResult self, string msg, string title = null)
        {
            self.ContentBuilder.Append($"ff.Alert('{msg}','{title??MvcProgram._localizer?["Sys.Info"]}');");
            return self;
        }

        /// <summary>
        /// 添加消息提示脚本
        /// </summary>
        /// <param name="self">FResult 实例</param>
        /// <param name="msg">提示消息内容</param>
        /// <param name="title">消息标题，为空时使用本地化的默认标题</param>
        /// <returns>返回当前 FResult 对象，支持链式调用</returns>
        public static FResult Message(this FResult self, string msg, string title = null)
        {
            self.ContentBuilder.Append($"ff.Msg('{msg}','{title??MvcProgram._localizer?["Sys.Info"]}');");
            return self;
        }


        /// <summary>
        /// 添加刷新指定窗口内表格数据的脚本
        /// </summary>
        /// <param name="self">FResult 实例</param>
        /// <param name="winId">窗口标识，为空时依次使用父窗口标识、主窗口容器标识</param>
        /// <param name="index">表格索引，用于区分同一页面中的多个表格</param>
        /// <returns>返回当前 FResult 对象，支持链式调用</returns>
        public static FResult RefreshGrid(this FResult self, string winId = "", int index = 0)
        {
            // 未指定窗口时优先取父窗口标识，仍取不到则回退到 LayUI 主页面容器标识
            if (string.IsNullOrEmpty(winId))
            {
                winId = self.Controller.ParentWindowId;
                if (string.IsNullOrEmpty(winId))
                {
                    winId = "LAY_app_body";
                }
            }
            self.ContentBuilder.Append($"ff.RefreshGrid('{winId}',{index});");
            return self;
        }

        /// <summary>
        /// Layui暂时不支持单行刷新，所以现在是 RefreshGrid
        /// </summary>
        /// <param name="self"></param>
        /// <param name="id"></param>
        /// <param name="winId"></param>
        /// <returns></returns>
        public static FResult RefreshGridRow(this FResult self, Guid id, string winId = "")
        {
            self.RefreshGrid(winId);
            return self;
        }

        /// <summary>
        /// 刷新指定行所在的表格（LayUI 暂不支持单行刷新，实际刷新整个表格）
        /// </summary>
        /// <param name="self">FResult 实例</param>
        /// <param name="id">行数据标识（字符串类型）</param>
        /// <param name="winId">窗口标识，为空时使用默认窗口</param>
        /// <returns>返回当前 FResult 对象，支持链式调用</returns>
        public static FResult RefreshGridRow(this FResult self, string id, string winId = "")
        {
            self.RefreshGrid(winId);
            return self;
        }

        /// <summary>
        /// 刷新指定行所在的表格（LayUI 暂不支持单行刷新，实际刷新整个表格）
        /// </summary>
        /// <param name="self">FResult 实例</param>
        /// <param name="id">行数据标识（长整型）</param>
        /// <param name="winId">窗口标识，为空时使用默认窗口</param>
        /// <returns>返回当前 FResult 对象，支持链式调用</returns>
        public static FResult RefreshGridRow(this FResult self, long id, string winId = "")
        {
            self.RefreshGrid(winId);
            return self;
        }
        /// <summary>
        /// 刷新当前页面
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static FResult RefreshPage(this FResult self)
        {
            self.ContentBuilder.Append($"layui.index.render();");
            return self;
        }

        /// <summary>
        /// 跳转浏览器页面
        /// </summary>
        /// <param name="self"></param>
        /// <param name="url">url</param>
        /// <returns></returns>
        public static FResult RedirectUrl(this FResult self, string url)
        {
            self.ContentBuilder.Append($"window.location.url='{url}'");
            return self;
        }


        /// <summary>
        /// 追加自定义的 JavaScript 脚本内容
        /// </summary>
        /// <param name="self">FResult 实例</param>
        /// <param name="script">自定义脚本内容</param>
        /// <returns>返回当前 FResult 对象，支持链式调用</returns>
        public static FResult AddCustomScript(this FResult self, string script)
        {
            self.ContentBuilder.Append(script);
            return self;
        }
    }
}

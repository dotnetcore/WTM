using System;
using WalkingTec.Mvvm.Core.Exceptions;

namespace WalkingTec.Mvvm.Mvc
{
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

        public static FResult Alert(this FResult self, string msg)
        {
            self.ContentBuilder.Append($"ff.Alert('{msg}');");
            return self;
        }

        public static FResult RefreshGrid(this FResult self, string winId = "", int index = 0)
        {
            if (string.IsNullOrEmpty(winId))
            {
                winId = self.Controller.ParentWindowId;
                if (string.IsNullOrEmpty(winId))
                {
                    winId = "DONOTUSE_MAINPANEL";
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
        /// 刷新页面
        /// </summary>
        /// <param name="self"></param>
        /// <param name="url">url 不允许为 NullOrEmpty</param>
        /// <returns></returns>
        public static FResult RefreshPage(this FResult self, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new NullOrEmptyStringException();
            }
            self.ContentBuilder.Append($"ff.LoadPage('{url}');");
            return self;
        }

        /// <summary>
        /// 默认刷新当前控制器下的 Index 页面
        /// </summary>
        /// <param name="self"></param>
        /// <param name="controller">当前控制器对象 传this即可</param>
        /// <returns></returns>
        public static FResult RefreshPage(this FResult self, BaseController controller)
        {
            var url = string.Empty;
            var routeVals = controller.RouteData.Values;

            //var url = $"{(string.IsNullOrEmpty()?)}/{routeVals["controller"]}/Index";
            if (routeVals.Keys.Contains("area"))
            {
                url += "/" + routeVals["area"];
            }

            //if (routeVals.Keys.Contains("controller"))
            //{
            //    url += "/" + routeVals["controller"];
            //}

            url += "/" + routeVals["controller"] + "/Index";

            return self.RefreshPage(url);
        }

        public static FResult AddCustomScript(this FResult self, string script)
        {
            self.ContentBuilder.Append(script);
            return self;
        }
    }
}

using System;

namespace WalkingTec.Mvvm.Core
{

    /// <summary>
    /// 列格式信息类，定义表格列单元格的显示格式，支持按钮、对话框、脚本、下载、图片预览、HTML等多种格式
    /// </summary>
    public class ColumnFormatInfo
    {
        /// <summary>
        /// 列格式类型
        /// </summary>
        public ColumnFormatTypeEnum FormatType { get; set; }

        /// <summary>
        /// 按钮类型
        /// </summary>
        public ButtonTypesEnum ButtonType { get; set; }
        /// <summary>
        /// 按钮或单元格显示文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 弹出窗口标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 点击按钮时执行的脚本(Script格式使用)
        /// </summary>
        public string Script { get; set; }

        /// <summary>
        /// 弹出窗口ID
        /// </summary>
        public string WindowID { get; set; }

        /// <summary>
        /// 按钮跳转或请求的Url地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 是否以对话框方式显示
        /// </summary>
        public bool ShowDialog { get; set; }

        /// <summary>
        /// 窗口是否可调整大小
        /// </summary>
        public bool Resizable { get; set; }

        /// <summary>
        /// 按钮ID
        /// </summary>
        public string ButtonID { get; set; }

        /// <summary>
        /// 窗口宽度
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// 窗口高度
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// 附件ID(下载、图片预览格式使用)
        /// </summary>
        public Guid? FileID { get; set; }

        /// <summary>
        /// 要显示的HTML内容(Html格式使用)
        /// </summary>
        public string Html { get; set; }

        /// <summary>
        /// 窗口是否默认最大化
        /// </summary>
        public bool Maxed { get; set; }

        /// <summary>
        /// 按钮CSS样式类
        /// </summary>
        public string ButtonClass { get; set; }

        /// <summary>
        /// 跳转类型(Button格式使用)
        /// </summary>
        public RedirectTypesEnum RType { get; set; }

        /// <summary>
        /// 按钮或单元格的CSS样式
        /// </summary>
        public string Style { get; set; }
        /// <summary>
        /// 创建一个弹出新窗口(Dialog)格式的列按钮
        /// </summary>
        /// <param name="buttonType">按钮类型</param>
        /// <param name="url">点击按钮后弹窗加载的Url地址</param>
        /// <param name="buttonText">按钮显示文本</param>
        /// <param name="width">弹窗宽度</param>
        /// <param name="height">弹窗高度</param>
        /// <param name="title">弹窗标题</param>
        /// <param name="buttonID">按钮ID</param>
        /// <param name="showDialog">是否以对话框方式显示</param>
        /// <param name="resizable">弹窗是否可调整大小</param>
        /// <param name="maxed">弹窗是否默认最大化</param>
        /// <param name="buttonclass">按钮CSS样式类</param>
        /// <param name="style">按钮CSS样式</param>
        /// <returns>对话框格式的列格式信息</returns>
        public static ColumnFormatInfo MakeDialogButton(ButtonTypesEnum buttonType, string url, string buttonText, int? width, int? height,  string title = null, string buttonID = null, bool showDialog = true, bool resizable = true, bool maxed = false,string buttonclass = null, string style=null)
        {
            ColumnFormatInfo rv = new ColumnFormatInfo();
            rv.FormatType = ColumnFormatTypeEnum.Dialog;
            rv.ButtonType = buttonType;
            rv.Url = url;
            rv.Width = width;
            rv.Height = height;
            rv.Text = buttonText;
            rv.Title = title;
            rv.ButtonID = buttonID;
            rv.ShowDialog = showDialog;
            rv.Resizable = resizable;
            rv.Maxed = maxed;
            rv.ButtonClass = buttonclass;
            rv.Style = style;
            return rv;
        }

        /// <summary>
        /// 创建一个点击后执行脚本(Script)的列按钮
        /// </summary>
        /// <param name="buttonType">按钮类型</param>
        /// <param name="url">按钮关联的Url地址</param>
        /// <param name="buttonText">按钮显示文本</param>
        /// <param name="buttonID">按钮ID</param>
        /// <param name="script">点击按钮后执行的脚本</param>
        /// <param name="buttonclass">按钮CSS样式类</param>
        /// <param name="style">按钮CSS样式</param>
        /// <returns>脚本格式的列格式信息</returns>
        public static ColumnFormatInfo MakeScriptButton(ButtonTypesEnum buttonType, string url, string buttonText, string buttonID = null, string script = "", string buttonclass = null, string style = null)
        {
            ColumnFormatInfo rv = new ColumnFormatInfo();
            rv.FormatType = ColumnFormatTypeEnum.Script;
            rv.ButtonType = buttonType;
            rv.Url = url;
            rv.Text = buttonText;
            rv.ButtonID = buttonID;
            rv.Script = script;
            rv.ButtonClass = buttonclass;
            rv.Style = style;
            return rv;
        }

        /// <summary>
        /// 创建一个普通跳转(Button)格式的列按钮
        /// </summary>
        /// <param name="buttonType">按钮类型</param>
        /// <param name="url">点击按钮后跳转的Url地址</param>
        /// <param name="buttonText">按钮显示文本</param>
        /// <param name="width">窗口宽度</param>
        /// <param name="height">窗口高度</param>
        /// <param name="title">窗口标题</param>
        /// <param name="buttonID">按钮ID</param>
        /// <param name="resizable">窗口是否可调整大小</param>
        /// <param name="maxed">窗口是否默认最大化</param>
        /// <param name="buttonclass">按钮CSS样式类</param>
        /// <param name="style">按钮CSS样式</param>
        /// <param name="rtype">跳转类型，默认为Layer弹层跳转</param>
        /// <returns>普通按钮格式的列格式信息</returns>
        public static ColumnFormatInfo MakeButton(ButtonTypesEnum buttonType, string url, string buttonText, int? width, int? height, string title = null, string buttonID = null, bool resizable = true, bool maxed = false, string buttonclass = null, string style = null, RedirectTypesEnum rtype = RedirectTypesEnum.Layer)
        {
            ColumnFormatInfo rv = new ColumnFormatInfo();
            rv.FormatType = ColumnFormatTypeEnum.Button;
            rv.ButtonType = buttonType;
            rv.Url = url;
            rv.Width = width;
            rv.Height = height;
            rv.Text = buttonText;
            rv.Title = title;
            rv.ButtonID = buttonID;
            rv.Resizable = resizable;
            rv.Maxed = maxed;
            rv.ButtonClass = buttonclass;
            rv.RType = rtype;
            rv.Style = style;
            return rv;
        }

        /// <summary>
        /// 创建一个下载附件(Download)格式的列按钮
        /// </summary>
        /// <param name="buttonType">按钮类型</param>
        /// <param name="fileID">要下载的附件ID</param>
        /// <param name="buttonText">按钮显示文本，为空时使用本地化的默认下载文本</param>
        /// <param name="buttonclass">按钮CSS样式类</param>
        /// <param name="style">按钮CSS样式</param>
        /// <returns>下载格式的列格式信息</returns>
        public static ColumnFormatInfo MakeDownloadButton(ButtonTypesEnum buttonType, Guid? fileID, string buttonText = null, string buttonclass = null, string style = null)
        {
            ColumnFormatInfo rv = new ColumnFormatInfo();
            rv.FormatType = ColumnFormatTypeEnum.Download;
            rv.ButtonType = buttonType;
            rv.FileID = fileID;
            rv.Text = buttonText?? CoreProgram._localizer?["Sys.Download"];
            rv.ButtonClass = buttonclass;
            rv.Style = style;
            return rv;
        }

        /// <summary>
        /// 创建一个预览附件图片(ViewPic)格式的列按钮
        /// </summary>
        /// <param name="buttonType">按钮类型</param>
        /// <param name="fileID">要预览的附件ID</param>
        /// <param name="width">预览窗口宽度</param>
        /// <param name="height">预览窗口高度</param>
        /// <param name="title">预览窗口标题，为空时使用本地化的默认预览标题</param>
        /// <param name="windowID">预览窗口ID</param>
        /// <param name="buttonText">按钮显示文本，为空时使用本地化的默认预览文本</param>
        /// <param name="resizable">预览窗口是否可调整大小</param>
        /// <param name="maxed">预览窗口是否默认最大化</param>
        /// <param name="buttonclass">按钮CSS样式类</param>
        /// <param name="style">按钮CSS样式</param>
        /// <returns>图片预览格式的列格式信息</returns>
        public static ColumnFormatInfo MakeViewButton(ButtonTypesEnum buttonType, Guid? fileID, int? width = null, int? height = null, string title = null, string windowID = null, string buttonText = null, bool resizable = true, bool maxed = false, string buttonclass = null, string style = null)
        {
            ColumnFormatInfo rv = new ColumnFormatInfo();
            rv.FormatType = ColumnFormatTypeEnum.ViewPic;
            rv.ButtonType = buttonType;
            rv.FileID = fileID;
            rv.Width = width;
            rv.Height = height;
            rv.WindowID = windowID;
            rv.Text = buttonText ?? CoreProgram._localizer?["Sys.Preview"];
            rv.Title = title ?? CoreProgram._localizer?["Sys.Preview"];
            rv.Resizable = resizable;
            rv.Maxed = maxed;
            rv.ButtonClass = buttonclass;
            rv.Style = style;
            return rv;
        }

        /// <summary>
        /// 创建一个直接显示HTML内容的列格式
        /// </summary>
        /// <param name="html">要显示的HTML内容</param>
        /// <returns>HTML格式的列格式信息</returns>
        public static ColumnFormatInfo MakeHtml(string html)
        {
            ColumnFormatInfo rv = new ColumnFormatInfo();
            rv.FormatType = ColumnFormatTypeEnum.Html;
            rv.Html = html;
            //内容非空时在末尾追加空的script标签，确保前端按HTML方式渲染单元格内容
            if (string.IsNullOrEmpty(rv.Html) == false)
            {
                rv.Html += "<script></script>";
            }
            return rv;
        }
    }
}

using System;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// UI服务接口，提供生成各类前端控件和按钮的统一方法，具体实现由不同UI框架提供
    /// </summary>
    public interface IUIService
    {
        /// <summary>
        /// 生成一个按钮，点击后按指定跳转方式打开页面
        /// </summary>
        /// <param name="buttonType">按钮类型</param>
        /// <param name="url">点击按钮后打开的页面地址</param>
        /// <param name="buttonText">按钮文字</param>
        /// <param name="width">弹出窗口宽度</param>
        /// <param name="height">弹出窗口高度</param>
        /// <param name="title">弹出窗口标题</param>
        /// <param name="buttonID">按钮ID</param>
        /// <param name="resizable">弹出窗口是否可调整大小</param>
        /// <param name="max">弹出窗口是否最大化</param>
        /// <param name="currentdivid">当前页面中层的ID</param>
        /// <param name="buttonClass">按钮样式类</param>
        /// <param name="style">按钮内联样式</param>
        /// <param name="rtype">跳转类型</param>
        /// <returns>生成的按钮Html代码</returns>
        string MakeButton(ButtonTypesEnum buttonType, string url, string buttonText, int? width, int? height, string title = null, string buttonID = null,bool resizable = true, bool max = false, string currentdivid = "", string buttonClass = null, string style = null, RedirectTypesEnum rtype = RedirectTypesEnum.Layer);
        /// <summary>
        /// 生成一个按钮，点击后弹出对话框页面
        /// </summary>
        /// <param name="buttonType">按钮类型</param>
        /// <param name="url">点击按钮后打开的页面地址</param>
        /// <param name="buttonText">按钮文字</param>
        /// <param name="width">弹出窗口宽度</param>
        /// <param name="height">弹出窗口高度</param>
        /// <param name="title">弹出窗口标题</param>
        /// <param name="buttonID">按钮ID</param>
        /// <param name="showDialog">是否直接弹出窗口</param>
        /// <param name="resizable">弹出窗口是否可调整大小</param>
        /// <param name="max">弹出窗口是否最大化</param>
        /// <param name="buttonClass">按钮样式类</param>
        /// <param name="style">按钮内联样式</param>
        /// <returns>生成的按钮Html代码</returns>
        string MakeDialogButton(ButtonTypesEnum buttonType, string url, string buttonText, int? width, int? height, string title = null, string buttonID = null, bool showDialog = true, bool resizable = true, bool max = false,string buttonClass = null, string style = null);

        /// <summary>
        /// 生成一个文件下载按钮
        /// </summary>
        /// <param name="buttonType">按钮类型</param>
        /// <param name="fileID">要下载的附件ID</param>
        /// <param name="buttonText">按钮文字</param>
        /// <param name="_DONOT_USE_CS">附件所在的连接串名称</param>
        /// <param name="buttonClass">按钮样式类</param>
        /// <param name="style">按钮内联样式</param>
        /// <returns>生成的按钮Html代码</returns>
        string MakeDownloadButton(ButtonTypesEnum buttonType, Guid fileID, string buttonText = null, string _DONOT_USE_CS = "default", string buttonClass = null, string style = null);

        /// <summary>
        /// 生成一个文件预览按钮，点击后弹出窗口查看附件
        /// </summary>
        /// <param name="buttonType">按钮类型</param>
        /// <param name="fileID">要预览的附件ID</param>
        /// <param name="buttonText">按钮文字</param>
        /// <param name="width">弹出窗口宽度</param>
        /// <param name="height">弹出窗口高度</param>
        /// <param name="title">弹出窗口标题</param>
        /// <param name="resizable">弹出窗口是否可调整大小</param>
        /// <param name="_DONOT_USE_CS">附件所在的连接串名称</param>
        /// <param name="maxed">弹出窗口是否最大化显示</param>
        /// <param name="buttonClass">按钮样式类</param>
        /// <param name="style">按钮内联样式</param>
        /// <returns>生成的按钮Html代码</returns>
        string MakeViewButton(ButtonTypesEnum buttonType, Guid fileID, string buttonText = null, int? width = null, int? height = null, string title = null, bool resizable = true, string _DONOT_USE_CS = "default", bool maxed = false, string buttonClass = null, string style = null);

        /// <summary>
        /// 生成一个脚本按钮，点击后执行指定的JavaScript脚本
        /// </summary>
        /// <param name="buttonType">按钮类型</param>
        /// <param name="buttonText">按钮文字</param>
        /// <param name="script">点击按钮时执行的脚本</param>
        /// <param name="buttonID">按钮ID</param>
        /// <param name="url">相关页面地址</param>
        /// <param name="buttonClass">按钮样式类</param>
        /// <param name="style">按钮内联样式</param>
        /// <returns>生成的按钮Html代码</returns>
        string MakeScriptButton(ButtonTypesEnum buttonType, string buttonText, string script = "", string buttonID = null, string url = null, string buttonClass = null, string style = null);

        /// <summary>
        /// 生成一个复选框控件
        /// </summary>
        /// <param name="ischeck">是否选中</param>
        /// <param name="text">显示文字</param>
        /// <param name="name">控件名称</param>
        /// <param name="value">控件值</param>
        /// <param name="isReadOnly">是否只读</param>
        /// <returns>生成的复选框Html代码</returns>
        string MakeCheckBox(bool ischeck, string text = null, string name = null, string value = null, bool isReadOnly = false);

        /// <summary>
        /// 生成一个单选框控件
        /// </summary>
        /// <param name="ischeck">是否选中</param>
        /// <param name="text">显示文字</param>
        /// <param name="name">控件名称</param>
        /// <param name="value">控件值</param>
        /// <param name="isReadOnly">是否只读</param>
        /// <returns>生成的单选框Html代码</returns>
        string MakeRadio(bool ischeck, string text = null, string name = null, string value = null, bool isReadOnly = false);

        /// <summary>
        /// 生成一个下拉框控件
        /// </summary>
        /// <param name="name">下拉框名称</param>
        /// <param name="value">下拉框数据源</param>
        /// <param name="selectedValue">选中的值</param>
        /// <param name="emptyText">空项显示的文字</param>
        /// <param name="isReadOnly">是否只读</param>
        /// <returns>生成的下拉框Html代码</returns>
        string MakeCombo(string name = null, List<ComboSelectListItem> value = null, string selectedValue = null, string emptyText = null, bool isReadOnly = false);

        /// <summary>
        /// 生成一个文本框控件
        /// </summary>
        /// <param name="name">文本框名称</param>
        /// <param name="value">文本框值</param>
        /// <param name="emptyText">文本框为空时显示的提示文字</param>
        /// <param name="isReadOnly">是否只读</param>
        /// <returns>生成的文本框Html代码</returns>
        string MakeTextBox(string name = null, string value = null, string emptyText = null, bool isReadOnly = false);
        /// <summary>
        /// 生成一个日期时间控件
        /// </summary>
        /// <param name="name">控件名称</param>
        /// <param name="value">控件值</param>
        /// <param name="emptyText">控件为空时显示的提示文字</param>
        /// <param name="isReadOnly">是否只读</param>
        /// <param name="dateType">日期类型（仅日期或日期时间）</param>
        /// <returns>生成的日期时间控件Html代码</returns>
        string MakeDateTime(string name = null, string value = null, string emptyText = null, bool isReadOnly = false, DateTimeTypeEnum? dateType = DateTimeTypeEnum.DateTime);
        
    }
}

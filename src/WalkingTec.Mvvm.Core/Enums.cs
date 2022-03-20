using System.ComponentModel.DataAnnotations;

namespace WalkingTec.Mvvm.Core
{
    public enum HttpMethodEnum
    {
        GET,
        POST,
        PUT,
        DELETE
    }


    /// <summary>
    /// 列表操作列类型
    /// </summary>
    public enum ColumnFormatTypeEnum
    {
        Dialog,//弹出窗口
        Button,//按钮
        Download,//下载
        ViewPic,//查看图片
        Script,//脚本
        Html
    }


    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBTypeEnum { SqlServer, MySql, PgSql, Memory, SQLite, Oracle }

    /// <summary>
    /// 页面显示方式
    /// </summary>
    public enum PageModeEnum { Single, Tab }

    /// <summary>
    /// Tab页的显示方式
    /// </summary>
    public enum TabModeEnum { Default, Simple }

    public enum BlazorModeEnum { Server, Wasm}

    /// <summary>
    /// 按钮
    /// </summary>
    public enum ButtonTypesEnum
    {
        Button,
        Link,
        Img
    };

    /// <summary>
    /// 按钮
    /// </summary>
    public enum RedirectTypesEnum
    {
        Layer,
        Self,
        NewWindow,
        NewTab,
    };

    /// <summary>
    /// 日期类型
    /// </summary>
    public enum DateTimeTypeEnum
    {
        /// <summary>
        /// 日期选择器
        /// 可选择：年、月、日
        /// </summary>
        Date,
        /// <summary>
        /// 日期时间选择器
        /// 可选择：年、月、日、时、分、秒
        /// </summary>
        DateTime,
        /// <summary>
        /// 年选择器
        /// 只提供年列表选择
        /// </summary>
        Year,
        /// <summary>
        /// 年月选择器
        /// 只提供年、月选择
        /// </summary>
        Month,
        /// <summary>
        /// 时间选择器
        /// 只提供时、分、秒选择
        /// </summary>
        Time
    };

    /// <summary>
    /// 图形枚举
    /// </summary>
    public enum ChartEnum
    {
        line,
        pie,
        column,
        bubble,
        barcolumn
    }

    /// <summary>
    /// 图形统计值类型
    /// </summary>
    public enum ChartValueType
    {
        sum,
        count,
        sumpct,
        countpct
    }
    /// <summary>
    /// 图形统计分区类型
    /// </summary>
    public enum PartitionType
    {
        year,
        month,
        day,
        hour,
        minute,
        second
    }

    public enum UIEnum
    { LayUI, React, VUE,Blazor }



    public enum BoolComboTypes { YesNo, ValidInvalid, MaleFemale, HaveNotHave, Custom }

    public enum SortDir { Asc, Desc }

    public enum BackgroudColorEnum
    {
        Grey,
        Yellow,
        Red
    };

    public enum GenderEnum
    {
        [Display(Name = "Sys.Male")]
        Male = 0,
        [Display(Name = "Sys.Female")]
        Female = 1
    }

}

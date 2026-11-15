using System.ComponentModel.DataAnnotations;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// Http 请求方式
    /// </summary>
    public enum HttpMethodEnum
    {
        /// <summary>
        /// GET 请求
        /// </summary>
        GET,
        /// <summary>
        /// POST 请求
        /// </summary>
        POST,
        /// <summary>
        /// PUT 请求
        /// </summary>
        PUT,
        /// <summary>
        /// DELETE 请求
        /// </summary>
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
        //输出原始Html
        Html
    }


    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBTypeEnum { SqlServer, MySql, PgSql, Memory, SQLite, Oracle,DaMeng }

    /// <summary>
    /// 页面显示方式
    /// </summary>
    public enum PageModeEnum { Single, Tab }

    /// <summary>
    /// Tab页的显示方式
    /// </summary>
    public enum TabModeEnum { Default, Simple }

    /// <summary>
    /// Blazor 运行模式
    /// </summary>
    public enum BlazorModeEnum { Server, Wasm}

    /// <summary>
    /// 按钮
    /// </summary>
    public enum ButtonTypesEnum
    {
        /// <summary>
        /// 普通按钮
        /// </summary>
        Button,
        /// <summary>
        /// 链接按钮
        /// </summary>
        Link,
        /// <summary>
        /// 图片按钮
        /// </summary>
        Img
    };

    /// <summary>
    /// 按钮
    /// </summary>
    public enum RedirectTypesEnum
    {
        /// <summary>
        /// 弹出层窗口
        /// </summary>
        Layer,
        /// <summary>
        /// 当前窗口跳转
        /// </summary>
        Self,
        /// <summary>
        /// 新窗口打开
        /// </summary>
        NewWindow,
        /// <summary>
        /// 新标签页打开
        /// </summary>
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
        /// <summary>
        /// 折线图
        /// </summary>
        line,
        /// <summary>
        /// 饼图
        /// </summary>
        pie,
        /// <summary>
        /// 柱状图
        /// </summary>
        column,
        /// <summary>
        /// 气泡图
        /// </summary>
        bubble,
        /// <summary>
        /// 条形图
        /// </summary>
        barcolumn
    }

    /// <summary>
    /// 图形统计值类型
    /// </summary>
    public enum ChartValueType
    {
        /// <summary>
        /// 求和
        /// </summary>
        sum,
        /// <summary>
        /// 计数
        /// </summary>
        count,
        /// <summary>
        /// 求和所占百分比
        /// </summary>
        sumpct,
        /// <summary>
        /// 计数所占百分比
        /// </summary>
        countpct
    }
    /// <summary>
    /// 图形统计分区类型
    /// </summary>
    public enum PartitionType
    {
        /// <summary>
        /// 按年分区
        /// </summary>
        year,
        /// <summary>
        /// 按月分区
        /// </summary>
        month,
        /// <summary>
        /// 按日分区
        /// </summary>
        day,
        /// <summary>
        /// 按小时分区
        /// </summary>
        hour,
        /// <summary>
        /// 按分钟分区
        /// </summary>
        minute,
        /// <summary>
        /// 按秒分区
        /// </summary>
        second
    }

    /// <summary>
    /// 前端 UI 框架类型
    /// </summary>
    public enum UIEnum
    { LayUI, React, VUE,Blazor,VUE3 }



    /// <summary>
    /// Bool 下拉框显示类型
    /// </summary>
    public enum BoolComboTypes { YesNo, ValidInvalid, MaleFemale, HaveNotHave, Custom }

    /// <summary>
    /// 排序方向
    /// </summary>
    public enum SortDir { Asc, Desc }

    /// <summary>
    /// 背景颜色
    /// </summary>
    public enum BackgroudColorEnum
    {
        /// <summary>
        /// 灰色
        /// </summary>
        Grey,
        /// <summary>
        /// 黄色
        /// </summary>
        Yellow,
        /// <summary>
        /// 红色
        /// </summary>
        Red
    };

    /// <summary>
    /// 性别
    /// </summary>
    public enum GenderEnum
    {
        /// <summary>
        /// 男
        /// </summary>
        [Display(Name = "Sys.Male")]
        Male = 0,
        /// <summary>
        /// 女
        /// </summary>
        [Display(Name = "Sys.Female")]
        Female = 1
    }

}

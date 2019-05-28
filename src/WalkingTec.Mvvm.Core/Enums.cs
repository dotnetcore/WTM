namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 列表操作列类型
    /// </summary>
    public enum ColumnFormatTypeEnum
    {
        Dialog,//弹出窗口
        Redirect,//转向
        Download,//下载
        ViewPic,//查看图片
        Script,//脚本
        Html
    }

    /// <summary>
    /// 上传图片存储方式
    /// </summary>
    public enum SaveFileModeEnum
    {
        Local = 0,//本地
        Database = 1,//数据库
        DFS = 2 //DFS
    };

    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBTypeEnum { SqlServer, MySql, PgSql,Memory }

    /// <summary>
    /// 页面显示方式
    /// </summary>
    public enum PageModeEnum { Single, Tab}

    /// <summary>
    /// Tab页的显示方式
    /// </summary>
    public enum TabModeEnum { Default, Simple}

    /// <summary>
    /// Notification出现的位置
    /// </summary>
    public enum ExtPosition
    {
        b = 0, //下
        bl = 1,//左下
        br = 2,//右下
        t = 3,//上
        tl = 4,//左上
        tr = 5,//右上
        l = 6,//左
        r = 7//右
    }
    /// <summary>
    /// Grid的选择模式
    /// </summary>
    public enum SelectionModeEnum
    {
        SINGLE,
        SIMPLE,
        MULTI
    };

    public enum SortType
    {
        Local,
        Remote,
        Disable
    }
    /// <summary>
    /// 按钮
    /// </summary>
    public enum ButtonTypesEnum
    {
        Button,
        Link
    };

    /// <summary>
    /// 按钮类型
    /// </summary>
    public enum ButtonOperationEnum
    {
        Submit,
        Button
    };

    ///// <summary>
    ///// 上传类型
    ///// </summary>
    //public enum UploadTypesEnum
    //{
    //    AllFiles,
    //    ImageFile,
    //    ZipFile,
    //    ExcelFile,
    //    WordFile,
    //    PDFFile,
    //    TextFile,
    //    Custom
    //};

    /// <summary>
    /// 日期类型
    /// </summary>
    public enum DateTimeTypeEnum
    {
        Date,
        Time,
        DateAndTime,
        Month
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
    {
        extjs,
        bootstrap
    }

    public enum NoRightEnum
    {
        /// <summary>
        /// 隐藏
        /// </summary>
        Invisible,
        /// <summary>
        /// 禁用
        /// </summary>
        Disable
    }

    public enum VTypeEnum { url, email, Date, Time, IPaddress, Int, UInt, Double, UDouble, Color, Phone, Tel }
    public enum HiddenModeEnum { Display, Visibility, Offsets }
    public enum RegionEnum { North, South, West, East, Center }
    public enum LayoutEnum { absolute, border, box, fit, center }
    public enum LabelAlignEnum { left, right, top }
    public enum DockDirEnum { top, left, right, bottom }
    public enum BoxAlignEnum { Stretch, Begin, Middle, End, StretchMax }
    public enum BoxPackEnum { Start, Middle, End }
    public enum BoxDirectionEnum { H, V }
    public enum MsgTargetEnum { qtip, title, under, side, none }
    public enum FieldTypeEnum { String, Int, Bool, Date }
    public enum IconAlignEnum { Left, Right, Bottom, Top }
    public enum UploadTypeEnum { AllFiles, ImageFile, ZipFile, ExcelFile, WordFile, PDFFile, TextFile }
    public enum ComponentRenderMode { Normal, Declare, Get, Reference }

    public enum BoolComboTypes { YesNo, ValidInvalid, MaleFemale, HaveNotHave, Custom }

    public enum SortDir { Asc, Desc }

    public enum BackgroudColorEnum
    {
        Grey,
        Yellow,
        Red
    };

}
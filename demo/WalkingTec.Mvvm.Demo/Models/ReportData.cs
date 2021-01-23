using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.Models
{
    public class WxReportData : PersistPoco
    {
        #region WxReportData Entity


        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "姓名")]
        public string ToWxUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "姓名")]
        public FrameworkUser FrameworkUser { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public Guid? FrameworkUserId { get; set; }


        [Display(Name = "报表日期")]
        public DateTime Date
        {
            get; set;
        }

        /// <summary>
        /// 报表类型
        /// </summary> 
        [Display(Name = "报表类型")]
        public WxMsgReport Type
        {
            get;
            set;
        }

        /// <summary>
        /// 数据类别-汇总/明细
        /// </summary> 
        [Display(Name = "数据类别")]
        public ReportDataType DataType { get; set; }


        #region MyRegion


        [Display(Name = "土方1:米")]

        public double TuFang1 { get; set; } = 0;


        [Display(Name = "距离1:KM")]

        public double KM1 { get; set; } = 0;

        [Display(Name = "土方2:米")]

        public double TuFang2 { get; set; } = 0;

        [Display(Name = "距离2:KM")]

        public double KM2 { get; set; } = 0;

        [Display(Name = "土方3:米")]
        public double TuFang3 { get; set; } = 0;

        [Display(Name = "距离3:KM")]

        public double KM3 { get; set; } = 0;
        [Display(Name = "土方4:米")]

        public double TuFang4 { get; set; } = 0;

        [Display(Name = "距离4:KM")]

        public double KM4 { get; set; } = 0;
        [Display(Name = "土方5:米")]

        public double TuFang5 { get; set; } = 0;

        [Display(Name = "距离5:KM")]

        public double KM5 { get; set; } = 0;
        [Display(Name = "土方6:米")]

        public double TuFang6 { get; set; } = 0;

        [Display(Name = "距离6:KM")]

        public double KM6 { get; set; } = 0;
        [Display(Name = "淤泥1:米")]

        public double YnNi1 { get; set; } = 0;

        [Display(Name = "淤泥1:KM")]

        public double YnNi1KM { get; set; } = 0;
        [Display(Name = "淤泥2:米")]

        public double YnNi2 { get; set; } = 0;
        [Display(Name = "淤泥2:KM")]

        public double YnNi2KM { get; set; } = 0;
        [Display(Name = "淤泥3:米")]

        public double YnNi3 { get; set; } = 0;
        [Display(Name = "淤泥3:KM")]

        public double YnNi3KM { get; set; } = 0;
        [Display(Name = "淤泥4:米")]

        public double YnNi4 { get; set; } = 0;
        [Display(Name = "淤泥1:KM")]

        public double YnNi4KM { get; set; } = 0;
        [Display(Name = "加油1:升")]
        public double JiaYou1 { get; set; } = 0;
        [Display(Name = "加油2:升")]
        public double JiaYou2 { get; set; } = 0;

        #endregion
        /// <summary>
        /// 就餐天数
        /// </summary>
        [Display(Name = "就餐天数")]
        public double CanFeiRen { get; set; } = 0;
        /// <summary>
        /// 借支金额
        /// </summary>
        [Display(Name = "借支金额")]
        public double JieZhiMoney { get; set; } = 0;

        #endregion;

        #region 扩展字段 保留未使用


        [Display(Name = "扩展字段")]

        public string Extend01
        {
            get;
            set;
        }

        [Display(Name = "扩展字段")]

        public string Extend02
        {
            get;
            set;
        }
        [Display(Name = "扩展字段")]

        public string Extend03
        {
            get;
            set;
        }
        [Display(Name = "扩展字段")]

        public string Extend04
        {
            get;
            set;
        }
        [Display(Name = "扩展字段")]

        public string Extend05
        {
            get;
            set;
        }
        [Display(Name = "扩展字段")]

        public Guid? Extend06
        {
            get;
            set;
        }
        [Display(Name = "扩展字段")]

        public int? Extend07
        {
            get;
            set;
        }
        [Display(Name = "扩展字段")]

        public int? Extend08
        {
            get;
            set;
        }
        [Display(Name = "扩展字段")]

        public double? Extend09
        {
            get;
            set;
        }
        [Display(Name = "扩展字段")]

        public double? Extend10
        {
            get;
            set;
        }
        #endregion
    }

    public class EnumWxMsg
    {
        /// <summary>
        /// 微信消息发送状态
        /// </summary>

        public enum WxMsgStatus
        {
            /// <summary>
            /// Ready = 0, 待发送
            /// </summary>
            [Display(Name = "待发送")]
            Ready = 0,
            /// <summary>
            /// Done = 2, 已发送
            /// </summary>
            [Display(Name = "已发送")]
            Done = 2,
            /// <summary>
            /// Error = 4, 异常
            /// </summary>
            [Display(Name = "异常")]
            Error = 4
        }

        /// <summary>
        /// 微信消息类型
        /// </summary>

        public enum WxPushType
        {
            /// <summary>
            /// Text = 1, 文本消息
            /// </summary>
            [Display(Name = "文本消息")]
            Text = 1,
            /// <summary>
            /// News = 2, 消息卡
            /// </summary>
            [Display(Name = "消息卡")]
            News = 2,


        }




    }

    public class ToTalReportPara
    {
        public DateTime sTime { get; set; }

        public DateTime eTime { get; set; }

        public Guid userID { get; set; }

        public WxMsgReport ReportType { get; set; }

        public bool isToTal { get; set; }
    }



    public enum WxMsgReport
    {

        /// <summary>
        /// 日报
        /// </summary>
        [Display(Name = "日报")]
        Day = 0,
        /// <summary>
        /// 周报 
        /// </summary>
        [Display(Name = "周报")]
        Week = 1,
        /// <summary>
        /// 月报
        /// </summary>
        [Display(Name = "月报")]
        Month = 2
    }
    public enum ReportDataType
    {

        /// <summary>
        /// 汇总
        /// </summary>
        [Display(Name = "汇总")]
        ToTal = 0,
        /// <summary>
        /// 明细 
        /// </summary>
        [Display(Name = "明细")]
        Detailed = 2,

    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    public enum ActionLogTypesEnum
    {
        [Display(Name = "普通")]
        Normal,
        [Display(Name = "异常")]
        Exception,
        [Display(Name = "调试")]
        Debug
    };

    /// <summary>
    /// ActionLog
    /// </summary>
    [Table("ActionLogs")]
    public class ActionLog : BasePoco, ICloneable
    {
        [Display(Name = "模块")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        public string ModuleName { get; set; }

        [Display(Name = "动作")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        public string ActionName { get; set; }

        [Display(Name = "ITCode")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        public string ITCode { get; set; }

        [Display(Name = "Url")]
        [StringLength(250, ErrorMessage = "{0}最多输入{1}个字符")]
        public string ActionUrl { get; set; }

        [Display(Name = "操作时间")]
        public DateTime ActionTime { get; set; }

        [Display(Name = "时长")]
        public double Duration { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        [Display(Name = "IP")]
        public string IP { get; set; }

        [Display(Name = "类型")]
        public ActionLogTypesEnum LogType { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        [NotMapped]
        [Display(Name = "Logs")]
        public List<Guid> Logs { get; set; }
    }


}

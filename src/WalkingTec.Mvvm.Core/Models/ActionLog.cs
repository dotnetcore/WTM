using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    public enum ActionLogTypesEnum
    {
        [Display(Name = "Normal")]
        Normal,
        [Display(Name = "Exception")]
        Exception,
        [Display(Name = "Debug")]
        Debug
    };

    /// <summary>
    /// ActionLog
    /// </summary>
    [Table("ActionLogs")]
    public class ActionLog : BasePoco, ICloneable
    {
        [Display(Name = "Module")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string ModuleName { get; set; }

        [Display(Name = "Action")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string ActionName { get; set; }

        [Display(Name = "Account")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string ITCode { get; set; }

        [Display(Name = "Url")]
        [StringLength(250, ErrorMessage = "{0}stringmax{1}")]
        public string ActionUrl { get; set; }

        [Display(Name = "ActionTime")]
        public DateTime ActionTime { get; set; }

        [Display(Name = "Duration")]
        public double Duration { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        [Display(Name = "IP")]
        public string IP { get; set; }

        [Display(Name = "LogType")]
        public ActionLogTypesEnum LogType { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public string GetLogString()
        {
            return $@"
|-{Core.Program._localizer["ActionTime"]}：{this.ActionTime}
|-{Core.Program._localizer["Account"]}：{this.ITCode??""}
|-IP：{this.IP??""}
|-{Core.Program._localizer["Module"]}：{this.ModuleName??""}
|-{Core.Program._localizer["MethodName"]}：{this.ActionName ?? ""}
|-Url：{this.ActionUrl ?? ""}
|-{Core.Program._localizer["Duration"]}：{this.Duration.ToString("F2")+" s"}
|-{Core.Program._localizer["Remark"]}：{this.Remark}
";
        }
    }


}

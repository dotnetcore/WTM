using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    public enum ActionLogTypesEnum
    {
        [Display(Name = "_Admin.Normal")]
        Normal,
        [Display(Name = "_Admin.Exception")]
        Exception,
        [Display(Name = "_Admin.Debug")]
        Debug,
        [Display(Name = "_Admin.Job")]
        Job
    };

    /// <summary>
    /// ActionLog
    /// </summary>
    [Table("ActionLogs")]
    public class ActionLog : BasePoco, ICloneable
    {
        [Display(Name = "_Admin.Module")]
        [StringLength(255, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string ModuleName { get; set; }

        [Display(Name = "_Admin.Action")]
        [StringLength(255, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string ActionName { get; set; }

        [Display(Name = "_Admin.Account")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string ITCode { get; set; }

        [Display(Name = "Url")]
        [StringLength(250, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string ActionUrl { get; set; }

        [Display(Name = "_Admin.ActionTime")]
        public DateTime ActionTime { get; set; }

        [Display(Name = "_Admin.Duration")]
        public double Duration { get; set; }

        [Display(Name = "_Admin.Remark")]
        public string Remark { get; set; }

        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Display(Name = "IP")]
        public string IP { get; set; }

        [Display(Name = "_Admin.LogType")]
        public ActionLogTypesEnum LogType { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public string GetLogString()
        {
            return $@"
|-{Core.CoreProgram._localizer?["_Admin.ActionTime"]}：{this.ActionTime}
|-{Core.CoreProgram._localizer?["_Admin.Account"]}：{this.ITCode??""}
|-IP：{this.IP??""}
|-{Core.CoreProgram._localizer?["_Admin.Module"]}：{this.ModuleName??""}
|-{Core.CoreProgram._localizer?["_Admin.MethodName"]}：{this.ActionName ?? ""}
|-Url：{this.ActionUrl ?? ""}
|-{Core.CoreProgram._localizer?["_Admin.Duration"]}：{this.Duration.ToString("F2")+" s"}
|-{Core.CoreProgram._localizer?["_Admin.Remark"]}：{this.Remark}
";
        }
    }


}

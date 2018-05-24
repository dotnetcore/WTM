using System;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.ActionLogVMs
{
    public class ActionLogDisplayModel : BasePoco
    {
        [Display(Name = "模块")]
        public string ModuleName { get; set; }

        [Display(Name = "动作")]
        public string ActionName { get; set; }

        [Display(Name = "ITCode")]
        public string ITCode { get; set; }

        [Display(Name = "Url")]
        public string ActionUrl { get; set; }

        [Display(Name = "操作时间")]
        public DateTime ActionTime { get; set; }

        [Display(Name = "耗时(s)")]
        public double Duration { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "IP")]
        public string IP { get; set; }

        [Display(Name = "类型")]
        public ActionLogTypesEnum LogType { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.ActionLogVMs
{
    public class ActionLogSearcher : BaseSearcher
    {

        [Display(Name = "ITCode")]
        public string ITCode { get; set; }

        [Display(Name = "Url")]
        public string ActionUrl { get; set; }

        [Display(Name = "类型")]
        public List<ActionLogTypesEnum> LogType { get; set; }

        [Display(Name = "时间(起)")]
        public DateTime? StartActionTime { get; set; }

        [Display(Name = "时间(止)")]
        public DateTime? EndActionTime { get; set; }

        [Display(Name = "IP")]
        public string IP { get; set; }
    }
}

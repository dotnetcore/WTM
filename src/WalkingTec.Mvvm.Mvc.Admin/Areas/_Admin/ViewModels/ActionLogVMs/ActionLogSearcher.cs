using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.ActionLogVMs
{
    public class ActionLogSearcher : BaseSearcher
    {

        [Display(Name = "Account")]
        public string ITCode { get; set; }

        [Display(Name = "Url")]
        public string ActionUrl { get; set; }

        [Display(Name = "LogType")]
        public List<ActionLogTypesEnum> LogType { get; set; }

        [Display(Name = "StartActionTime")]
        public DateTime? StartActionTime { get; set; }

        [Display(Name = "EndActionTime")]
        public DateTime? EndActionTime { get; set; }

        [Display(Name = "IP")]
        public string IP { get; set; }
    }
}

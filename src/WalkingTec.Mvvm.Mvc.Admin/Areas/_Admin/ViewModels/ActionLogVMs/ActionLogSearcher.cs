using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
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

        [Display(Name = "ActionTime")]
        public DateRange ActionTime { get; set; }


        [Display(Name = "IP")]
        public string IP { get; set; }

        [Display(Name = "Duration")]
        public double? Duration { get; set; }

        public override void Validate()
        {
            if (ITCode == "单一错误")
            {
                MSD.AddModelError("ITCode", "itcode错误");
            }
            else if (ITCode == "全局错误")
            {
                MSD.AddModelError(" ", "全局错误");
            }
            else if (ITCode == "多个错误")
            {
                MSD.AddModelError("ITCode", "itcode错误");
                MSD.AddModelError(" ", "全局错误");
            }
            base.Validate();
        }
    }
}

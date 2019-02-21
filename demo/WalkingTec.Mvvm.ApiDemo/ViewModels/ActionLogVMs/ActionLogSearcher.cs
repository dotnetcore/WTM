using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Core;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.ActionLogVMs
{
    public class ActionLogSearcher : BaseSearcher
    {
        [Display(Name = "ITCode")]
        public String ITCode { get; set; }
        [Display(Name = "Url")]
        public String ActionUrl { get; set; }
        [Display(Name = "类型")]
        public ActionLogTypesEnum LogType { get; set; }

        protected override void InitVM()
        {
        }

    }
}

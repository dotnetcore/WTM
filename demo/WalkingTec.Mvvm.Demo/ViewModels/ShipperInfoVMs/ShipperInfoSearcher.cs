using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo;


namespace WalkingTec.Mvvm.Demo.ViewModels.ShipperInfoVMs
{
    public class ShipperInfoSearcher : BaseSearcher
    {
        [Display(Name = "发件人code")]
        public String shipper_code { get; set; }
        [Display(Name = "发件人姓名")]
        public String shipper_name { get; set; }

        protected override void InitVM()
        {
        }

    }
}

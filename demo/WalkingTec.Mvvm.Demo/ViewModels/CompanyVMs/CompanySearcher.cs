using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.CompanyVMs
{
    public class CompanySearcher : BaseSearcher
    {
        [Display(Name = "公司名称")]
        public String Name { get; set; }
        [Display(Name = "公司描述")]
        public String Description { get; set; }

        protected override void InitVM()
        {
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Doc.Models;


namespace WebApplication1.aaa.ViewModels.SchoolVMs
{
    public class SchoolSearcher : BaseSearcher
    {
        [Display(Name = "学校编码")]
        public String SchoolCode { get; set; }
        [Display(Name = "备注")]
        public String Remark { get; set; }

        protected override void InitVM()
        {
        }

    }
}

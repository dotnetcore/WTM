﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.ReactDemo.ViewModels.MajorVMs
{
    public partial class MajorSearcher : BaseSearcher
    {
        [Display(Name = "专业编码")]
        public String MajorCode { get; set; }
        [Display(Name = "专业名称")]
        public String MajorName { get; set; }
        [Display(Name = "专业类别")]
        public MajorTypeEnum? MajorType { get; set; }
        [Display(Name = "所属学校")]
        public int? SchoolId { get; set; }

        protected override async Task InitVM()
        {
        }

    }
}

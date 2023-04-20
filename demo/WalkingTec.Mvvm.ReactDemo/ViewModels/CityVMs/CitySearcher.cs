﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.ReactDemo.ViewModels.CityVMs
{
    public partial class CitySearcher : BaseSearcher
    {
        [Display(Name = "_Admin.Parent")]
        public Guid? ParentId { get; set; }

        protected override async Task InitVM()
        {
        }

    }
}

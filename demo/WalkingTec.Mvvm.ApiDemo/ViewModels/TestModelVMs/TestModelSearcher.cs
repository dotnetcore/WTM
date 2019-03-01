using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ApiDemo;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.TestModelVMs
{
    public class TestModelSearcher : BaseSearcher
    {
        public Decimal? Teset { get; set; }

        protected override void InitVM()
        {
        }

    }
}

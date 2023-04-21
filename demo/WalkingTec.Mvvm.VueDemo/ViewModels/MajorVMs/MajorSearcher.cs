using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.VueDemo.ViewModels.MajorVMs
{
    public partial class MajorSearcher : BaseSearcher
    {

        protected override Task InitVM()
        {
            return Task.CompletedTask;
        }

    }
}

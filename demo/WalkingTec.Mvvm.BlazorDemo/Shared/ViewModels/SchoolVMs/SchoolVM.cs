using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.BlazorDemo.Models;
using WalkingTec.Mvvm.BlazorDemo.ViewModels.MajorVMs;

namespace WalkingTec.Mvvm.BlazorDemo.ViewModels.SchoolVMs
{
    public class SchoolVM : BaseCRUDVM<School>
    {
        public MajorDetailListVM MajorList { get; set; }


        public SchoolVM()
        {
            MajorList = new MajorDetailListVM();
            SetInclude(x => x.Photos);
        }


        protected override void InitVM()
        {
            MajorList.CopyContext(this);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo.ViewModels.MajorVMs;

namespace WalkingTec.Mvvm.Demo.ViewModels.SchoolVMs
{
    public class SchoolVM : BaseCRUDVM<School>
    {
        public MajorDetailListVM MajorList { get; set; }

        public SchoolVM()
        {
            MajorList = new MajorDetailListVM();
            SetInclude(x => x.Photos, x=>x.Files);
        }


        protected override Task InitVM()
        {
            MajorList.CopyContext(this);
            return Task.CompletedTask;
        }

    }
}

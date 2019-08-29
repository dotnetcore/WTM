using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;
using WalkingTec.Mvvm.Demo.ViewModels.MajorVMs;

namespace WalkingTec.Mvvm.Demo.ViewModels.SchoolVMs
{
    public class SchoolVM : BaseCRUDVM<School>
    {
        public MajorDetailListVM MajorList { get; set; }

        public int? Slider0 { get; set; }
        public int? Slider1 { get; set; }
        public int? Slider2 { get; set; }

        public List<ComboSelectListItem> TransferItmes { get; set; }

        public Guid[] SchoolIds { get; set; }


        public SchoolVM()
        {
            MajorList = new MajorDetailListVM();
        }

        protected override void InitVM()
        {
            var ss = DC.Set<School>().ToList();
            TransferItmes = DC.Set<School>().GetSelectListItems(null, null, y => y.SchoolName);

            MajorList.CopyContext(this);
        }

    }
}

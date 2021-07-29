using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.LinkTest2VMs
{
    public partial class LinkTest2Searcher : BaseSearcher
    {
        public String name { get; set; }
        public List<ComboSelectListItem> AllLinkStudents { get; set; }
        public List<string> SelectedLinkStudentIDs { get; set; }

        protected override void InitVM()
        {
            AllLinkStudents = DC.Set<Student>().GetSelectListItems(Wtm, y => y.Name);
        }

    }
}

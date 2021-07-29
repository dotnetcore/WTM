using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.LinkTestVMs
{
    public partial class LinkTestSearcher : BaseSearcher
    {
        public String name { get; set; }
        public List<ComboSelectListItem> AllStudents { get; set; }
        public string StudentId { get; set; }

        protected override void InitVM()
        {
            AllStudents = DC.Set<Student>().GetSelectListItems(Wtm, y => y.Name);
        }

    }
}

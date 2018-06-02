using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Doc.Models;

namespace WalkingTec.Mvvm.Doc.ViewModels.MajorVms
{
    public class MajorVm : BaseCRUDVM<Major>
    {
        public string SchoolName { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.ViewModels.DepartmentVMs
{
    public class DepartmentSearcher : BaseSearcher
    {
        [Display(Name = "部门名称")]
        public String DepName { get; set; }

        protected override void InitVM()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.MajorVMs
{
    public class MajorSearcher : BaseSearcher
    {
        [Display(Name = "专业编码")]
        public String MajorCode { get; set; }
        [Display(Name = "专业名称")]
        public String MajorName { get; set; }
        public List<ComboSelectListItem> AllSchools { get; set; }
        [Display(Name = "所属学校")]
        public long? SchoolId { get; set; }

        protected override void InitVM()
        {
            AllSchools = DC.Set<School>().GetSelectListItems(LoginUserInfo.DataPrivileges, null, y => y.SchoolName);
        }

    }
}

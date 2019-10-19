using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs
{
    public enum DpTypeEnum
    {
        [Display(Name = "GroupDp")]
        UserGroup,
        [Display(Name = "UserDp")]
        User
    }

    public class DataPrivilegeSearcher : BaseSearcher
    {
        [Display(Name = "Account")]
        public string Name { get; set; }
        [Display(Name = "Privileges")]
        public string TableName { get; set; }
        public List<ComboSelectListItem> TableNames { get; set; }

        [Display(Name = "DpType")]
        public DpTypeEnum DpType { get; set; }
        public Guid? DomainID { get; set; }
        public List<ComboSelectListItem> AllDomains { get; set; }
        protected override void InitVM()
        {
            TableNames = new List<ComboSelectListItem>();
        }
    }
}

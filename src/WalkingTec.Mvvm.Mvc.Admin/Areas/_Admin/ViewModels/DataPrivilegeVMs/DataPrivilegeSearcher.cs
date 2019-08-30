using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs
{
    public enum DpTypeEnum
    {
        [Display(Name = "用户组权限")]
        UserGroup,
        [Display(Name = "用户权限")]
        User
    }

    public class DataPrivilegeSearcher : BaseSearcher
    {
        [Display(Name = "ITCode")]
        public string Name { get; set; }
        [Display(Name = "权限")]
        public string TableName { get; set; }
        public List<ComboSelectListItem> TableNames { get; set; }

        [Display(Name = "权限类别")]
        public DpTypeEnum DpType { get; set; }
        [Display(Name = "域名")]
        public Guid? DomainID { get; set; }
        public List<ComboSelectListItem> AllDomains { get; set; }
        protected override void InitVM()
        {
            TableNames = new List<ComboSelectListItem>();
        }
    }
}
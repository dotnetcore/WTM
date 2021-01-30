// WTM默认页面 Wtm buidin page
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.DataPrivilegeVMs
{
    public enum DpTypeEnum
    {
        [Display(Name = "_Admin.GroupDp")]
        UserGroup,
        [Display(Name = "_Admin.UserDp")]
        User
    }

    public class DataPrivilegeSearcher : BaseSearcher
    {
        [Display(Name = "_Admin.Account")]
        public string Name { get; set; }
        [Display(Name = "_Admin.Privileges")]
        public string TableName { get; set; }
        public List<ComboSelectListItem> TableNames { get; set; }

        [Display(Name = "_Admin.DpType")]
        public DpTypeEnum DpType { get; set; }
        public Guid? DomainID { get; set; }
        public List<ComboSelectListItem> AllDomains { get; set; }
        protected override void InitVM()
        {
            TableNames = new List<ComboSelectListItem>();
        }
    }
}

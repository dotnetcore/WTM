using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkMenuVMs
{
    public enum FrameworkMenuMode { Normal = 0, RoleMode = 1, RoleSetMode = 2 }
    public class FrameworkMenuSearcher : BaseSearcher
    {
        [Display(Name = "页面名称")]
        public string PageName { get; set; }
        [Display(Name = "模块名称")]
        public string ModuleName { get; set; }
        [Display(Name = "动作名称")]
        public string ActionName { get; set; }
        [Display(Name = "菜单显示")]
        public bool? ShowOnMenu { get; set; }
        [Display(Name = "公共页面")]
        public bool? IsPublic { get; set; }
        [Display(Name = "是否目录")]
        public bool? FolderOnly { get; set; }

        public Guid? RoleID { get; set; }
    }
}

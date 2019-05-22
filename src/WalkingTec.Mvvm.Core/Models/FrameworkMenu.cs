using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkMenu
    /// </summary>
    [Table("FrameworkMenus")]
    public class FrameworkMenu : BasePoco, ITreeData<FrameworkMenu>
    {

        [Display(Name = "页面名称")]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        [Required(ErrorMessage ="{0}是必填项")]
        public string PageName { get; set; }

        [Display(Name = "动作名称")]
        public string ActionName { get; set; }

        [Display(Name = "模块名称")]
        public string ModuleName { get; set; }

        [Display(Name = "目录" )]
        [Required(ErrorMessage ="{0}是必填项")]
        public bool FolderOnly { get; set; }

        [Display(Name = "继承" )]
        [Required(ErrorMessage ="{0}是必填项")]
        public bool IsInherit { get; set; }

        [Display(Name = "权限" )]
        public List<FunctionPrivilege> Privileges { get; set; }

        public string ClassName { get; set; }
        public string MethodName { get; set; }
        //[Display(Name = "动作" )]
        //public Guid? ActionId { get; set; }

        //[Display(Name = "模块" )]
        //public Guid? ModuleId { get; set; }

        [Display(Name = "域" )]
        public Guid? DomainId { get; set; }

        [Display(Name = "域" )]
        public FrameworkDomain Domain { get; set; }

        [Display(Name = "菜单显示" )]
        [Required(ErrorMessage ="{0}是必填项")]
        public bool ShowOnMenu { get; set; }

        [Display(Name = "公开" )]
        [Required(ErrorMessage ="{0}是必填项")]
        public bool IsPublic { get; set; }

        [Display(Name = "顺序" )]
        [Required(ErrorMessage ="{0}是必填项")]
        public int? DisplayOrder { get; set; }

        [Display(Name = "内部地址" )]
        [Required(ErrorMessage ="{0}是必填项")]
        public bool? IsInside { get; set; }
        
        public string Url { get; set; }

        /// <summary>
        /// 菜单图标路径
        /// </summary>
        [Display(Name = "图标" )]
        public FileAttachment ICon { get; set; }

        [Display(Name = "图标")]
        public Guid? IConId { get; set; }

        [Display(Name = "自定义图标")]
        public string CustumIcon { get; set; }

        #region ITreeData成员

        [Display(Name = "父目录" )]
        public Guid? ParentId { get; set; }
        [Display(Name = "父目录")]
        [JsonIgnore]
        public FrameworkMenu Parent { get; set; }
        [Display(Name = "子项" )]
        [JsonIgnore]
        public List<FrameworkMenu> Children { get; set; }

        #endregion

    }
}

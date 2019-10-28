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

        [Display(Name = "PageName")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        [Required(ErrorMessage = "{0}required")]
        public string PageName { get; set; }

        [Display(Name = "ActionName")]
        public string ActionName { get; set; }

        [Display(Name = "ModuleName")]
        public string ModuleName { get; set; }

        [Display(Name = "FolderOnly")]
        [Required(ErrorMessage = "{0}required")]
        public bool FolderOnly { get; set; }

        [Display(Name = "IsInherit")]
        [Required(ErrorMessage = "{0}required")]
        public bool IsInherit { get; set; }

        [Display(Name = "Privileges")]
        public List<FunctionPrivilege> Privileges { get; set; }

        /// <summary>
        /// ClassName
        /// </summary>
        /// <value></value>
        public string ClassName { get; set; }

        /// <summary>
        /// MethodName
        /// </summary>
        /// <value></value>
        public string MethodName { get; set; }

        [Display(Name = "Domain")]
        public Guid? DomainId { get; set; }

        [Display(Name = "Domain")]
        public FrameworkDomain Domain { get; set; }

        [Display(Name = "ShowOnMenu")]
        [Required(ErrorMessage = "{0}required")]
        public bool ShowOnMenu { get; set; }

        [Display(Name = "IsPublic")]
        [Required(ErrorMessage = "{0}required")]
        public bool IsPublic { get; set; }

        [Display(Name = "DisplayOrder")]
        [Required(ErrorMessage = "{0}required")]
        public int? DisplayOrder { get; set; }

        [Display(Name = "IsInside")]
        [Required(ErrorMessage = "{0}required")]
        public bool? IsInside { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        /// <value></value>
        public string Url { get; set; }

        [Display(Name = "ICon")]
        [StringLength(50)]
        public string ICon { get; set; }

        #region ITreeData成员

        [Display(Name = "ParentFolder")]
        public Guid? ParentId { get; set; }
        [Display(Name = "ParentFolder")]
        [JsonIgnore]
        public FrameworkMenu Parent { get; set; }
        [Display(Name = "Children")]
        [JsonIgnore]
        public List<FrameworkMenu> Children { get; set; }

        #endregion

    }
}

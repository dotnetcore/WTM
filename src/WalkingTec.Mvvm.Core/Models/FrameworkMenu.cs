using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkMenu
    /// </summary>
    [Table("FrameworkMenus")]
    public class FrameworkMenu : TreePoco<FrameworkMenu>
    {

        [Display(Name = "_Admin.PageName")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public string PageName { get; set; }

        [Display(Name = "_Admin.ActionName")]
        public string ActionName { get; set; }

        [Display(Name = "Codegen.ModuleName")]
        public string ModuleName { get; set; }

        [Display(Name = "_Admin.FolderOnly")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public bool FolderOnly { get; set; }

        [Display(Name = "_Admin.IsInherit")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public bool IsInherit { get; set; }

        [Display(Name = "_Admin.Privileges")]
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

        [Display(Name = "_Admin.Domain")]
        public string Domain { get; set; }

        [Display(Name = "_Admin.ShowOnMenu")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public bool ShowOnMenu { get; set; }

        [Display(Name = "_Admin.IsPublic")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public bool IsPublic { get; set; }

        [Display(Name = "_Admin.DisplayOrder")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public int? DisplayOrder { get; set; }

        [Display(Name = "_Admin.IsInside")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public bool? IsInside { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        /// <value></value>
        public string Url { get; set; }

        [Display(Name = "_Admin.Icon")]
        [StringLength(50)]
        public string Icon { get; set; }

    }
}

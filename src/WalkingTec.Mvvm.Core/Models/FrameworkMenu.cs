using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkMenu
    /// </summary>
    [Table("FrameworkMenus")]
    public class FrameworkMenu : TreePoco<FrameworkMenu>
    {

        /// <summary>
        /// 菜单页面名称，显示在系统菜单中
        /// </summary>
        [Display(Name = "_Admin.PageName")]
        [StringLength(100, ErrorMessage = "Validate.{0}stringmax{1}")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public string PageName { get; set; }

        /// <summary>
        /// 动作名称，即页面上按钮或功能点的名称，用于功能权限控制
        /// </summary>
        [Display(Name = "_Admin.ActionName")]
        public string ActionName { get; set; }

        /// <summary>
        /// 所属模块（控制器）名称
        /// </summary>
        [Display(Name = "Codegen.ModuleName")]
        public string ModuleName { get; set; }

        /// <summary>
        /// 是否为纯文件夹节点，仅用于菜单分组，不对应实际页面
        /// </summary>
        [Display(Name = "_Admin.FolderOnly")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public bool FolderOnly { get; set; }

        /// <summary>
        /// 子节点是否继承父节点的权限设置
        /// </summary>
        [Display(Name = "_Admin.IsInherit")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public bool IsInherit { get; set; }

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

        /// <summary>
        /// 所属域，用于多数据库/多域场景下的权限隔离
        /// </summary>
        [Display(Name = "_Admin.Domain")]
        public string Domain { get; set; }

        /// <summary>
        /// 是否显示在系统菜单上
        /// </summary>
        [Display(Name = "_Admin.ShowOnMenu")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public bool ShowOnMenu { get; set; }

        /// <summary>
        /// 是否为公开页面，公开页面无需授权即可访问
        /// </summary>
        [Display(Name = "_Admin.IsPublic")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// 显示顺序，数值越小排列越靠前
        /// </summary>
        [Display(Name = "_Admin.DisplayOrder")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public int? DisplayOrder { get; set; }

        /// <summary>
        /// 是否为系统内置页面
        /// </summary>
        [Display(Name = "_Admin.IsInside")]
        [Required(ErrorMessage = "Validate.{0}required")]
        public bool? IsInside { get; set; }

        /// <summary>
        /// 是否允许租户使用该菜单
        /// </summary>
        [Display(Name = "_Admin.TenantAllowed")]
        public bool? TenantAllowed { get; set; }


        /// <summary>
        /// Url
        /// </summary>
        /// <value></value>
        public string Url { get; set; }

        /// <summary>
        /// 菜单图标样式名称
        /// </summary>
        [Display(Name = "_Admin.Icon")]
        [StringLength(50)]
        public string Icon { get; set; }
    }
}

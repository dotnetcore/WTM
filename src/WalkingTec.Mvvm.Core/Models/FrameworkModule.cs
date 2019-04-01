using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkModule
    /// </summary>
    [Table("FrameworkModules")]
    public class FrameworkModule : BasePoco
    {
        [Display(Name = "模块名称")]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        [Required(ErrorMessage ="{0}是必填项")]
        public string ModuleName { get; set; }

        [Required(ErrorMessage ="{0}是必填项")]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        [Display(Name = "类名")]
        public string ClassName { get; set; }

        [Display(Name = "动作")]
        public List<FrameworkAction> Actions { get; set; }

        [Display(Name = "区域")]
        public Guid? AreaId { get; set; }
        [Display(Name = "区域")]
        public FrameworkArea Area { get; set; }

        public string NameSpace { get; set; }

        [NotMapped]
        public bool IgnorePrivillege { get; set; }

        [NotMapped]
        public bool IsApi { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return this.NameSpace + "," + this.ClassName;
            }
        }
    }
}

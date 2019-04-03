using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkAction
    /// </summary>
    [Table("FrameworkActions")]
    public class FrameworkAction : BasePoco
    {
        [Display(Name = "动作名称")]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        [Required(ErrorMessage ="{0}是必填项")]
        public string ActionName { get; set; }

        [Required(ErrorMessage ="{0}是必填项")]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        [Display(Name = "方法" )]
        public string MethodName { get; set; }

        [Display(Name = "模块" )]
        public Guid? ModuleId { get; set; }

        [Display(Name = "模块" )]
        public FrameworkModule Module { get; set; }

        [Display(Name = "参数" )]
        [StringLength(50,ErrorMessage ="{0}最多输入{1}个字符")]
        public string Parameter { get; set; }

        [NotMapped]
        public List<string> ParasToRunTest { get; set; }

        [NotMapped]
        public bool IgnorePrivillege { get; set; }

        [NotMapped]
        private string _url;
        [NotMapped]
        public string Url
        {
            get
            {
                if (_url == null)
                {
                    if (this.Module.Area != null)
                    {
                        _url = "/" + this.Module.Area.Prefix + "/" + this.Module.ClassName + "/" + this.MethodName;
                    }
                    else
                    {
                        _url = "/" + this.Module.ClassName + "/" + this.MethodName;
                    }
                }
                return _url;
            }
            set
            {
                _url = value;
            }
        }
    }

}

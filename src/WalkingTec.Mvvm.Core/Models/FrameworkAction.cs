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
        public ActionDescriptionAttribute ActionDes { get; set; }
        public string _name;
        public string ActionName
        {
            get
            {
                if (ActionDes?._localizer != null && string.IsNullOrEmpty(ActionDes?.Description) == false)
                {
                    if (ActionDes._localizer[ActionDes.Description].ResourceNotFound == true)
                    {
                        return Core.Program._localizer[ActionDes.Description];
                    }
                    else
                    {
                        return ActionDes._localizer[ActionDes.Description];
                    }


                }
                else
                {
                    return _name ?? "";
                }
            }
            set
            {
                _name = value;
            }
        }
        [Required(ErrorMessage ="{0}required")]
        [StringLength(50,ErrorMessage = "{0}stringmax{1}")]
        [Display(Name = "MethodName")]
        public string MethodName { get; set; }

        [Display(Name = "Module")]
        public Guid? ModuleId { get; set; }

        [Display(Name = "Module")]
        public FrameworkModule Module { get; set; }

        [Display(Name = "Parameter")]
        [StringLength(50,ErrorMessage = "{0}stringmax{1}")]
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

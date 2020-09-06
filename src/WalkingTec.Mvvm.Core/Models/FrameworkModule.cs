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

        public ActionDescriptionAttribute ActionDes { get; set; }
        public string _name;
        public string ModuleName
        {
            get
            {
                string rv = "";
                if (ActionDes?._localizer != null && string.IsNullOrEmpty(ActionDes?.Description) == false)
                {
                    if (ActionDes._localizer[ActionDes.Description].ResourceNotFound == true)
                    {
                        rv =  Core.Program._localizer[ActionDes.Description];
                    }
                    else
                    {
                        rv = ActionDes._localizer[ActionDes.Description];
                    }
                }
                else
                {
                    rv = _name ?? "";
                }

                if (IsApi == true)
                {
                    rv += "(api)";
                }
                return rv;

            }
            set
            {
                _name = value;
            }
        }



        [Required(ErrorMessage ="{0}required")]
        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        [Display(Name = "ClassName")]
        public string ClassName { get; set; }

        [Display(Name = "Action")]
        public List<FrameworkAction> Actions { get; set; }

        [Display(Name = "Area")]
        public Guid? AreaId { get; set; }
        [Display(Name = "Area")]
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

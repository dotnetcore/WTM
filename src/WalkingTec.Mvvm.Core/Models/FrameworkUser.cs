using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkUser
    /// </summary>
    [Table("FrameworkUsers")]
    public  class FrameworkUserBase : BasePoco
    {
        [Display(Name = "Account")]
        [Required(ErrorMessage = "{0}required")]
        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        public string ITCode { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}required")]
        [StringLength(32, ErrorMessage = "{0}stringmax{1}")]
        public string Password { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "{0}required")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string Name { get; set; }

        [Display(Name = "IsValid")]
        public bool IsValid { get; set; }

        [Display(Name = "Photo")]
        public Guid? PhotoId { get; set; }

        [Display(Name = "Photo")]
        public FileAttachment Photo { get; set; }

    }
}

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
    public  abstract class FrameworkUserBase : BasePoco
    {
        [Display(Name = "_Admin.Account")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50,ErrorMessage = "Validate.{0}stringmax{1}")]
        public string ITCode { get; set; }

        [Display(Name = "_Admin.Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Validate.{0}required")]
        [StringLength(32, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string Password { get; set; }

        [Display(Name = "_Admin.Name")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string Name { get; set; }

        [Display(Name = "_Admin.IsValid")]
        public bool IsValid { get; set; }

        [Display(Name = "_Admin.Photo")]
        public Guid? PhotoId { get; set; }

        [Display(Name = "_Admin.Photo")]
        [JsonIgnore]
        public FileAttachment Photo { get; set; }

        [Display(Name = "_Admin.Tenant")]
        public string TenantCode { get; set; }
    }
}

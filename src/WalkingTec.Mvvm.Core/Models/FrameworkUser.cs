using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace WalkingTec.Mvvm.Core
{
    public enum SexEnum
    {
        [Display(Name = "Male")]
        Male = 0,
        [Display(Name = "Female")]
        Female = 1
    }
    /// <summary>
    /// FrameworkUser
    /// </summary>
    [Table("FrameworkUsers")]
    public class FrameworkUserBase : BasePoco
    {
        [Display(Name = "Account")]
        [Required(ErrorMessage = "{0}required")]
        [StringLength(50,ErrorMessage ="{0}stringmax{1}")]
        public string ITCode { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0}required")]
        [StringLength(32, ErrorMessage = "{0}stringmax{1}")]
        public string Password { get; set; }

        [Display(Name = "Email" )]
        [RegularExpression("^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "{0}formaterror")]
        [StringLength(50,ErrorMessage = "{0}stringmax{1}")]
        public string Email { get; set; }

        [Display(Name = "Name" )]
        [Required(ErrorMessage = "{0}required")]
        [StringLength(50,ErrorMessage = "{0}stringmax{1}")]
        public string Name { get; set; }

        [Display(Name = "Sex")]
        public SexEnum? Sex { get; set; }

        [Display(Name = "CellPhone")]
        [RegularExpression("^[1][3-9]\\d{9}$", ErrorMessage = "{0}formaterror")]
        public string CellPhone { get; set; }

        [Display(Name = "HomePhone")]
        [StringLength(30, ErrorMessage = "{0}stringmax{1}")]
        [RegularExpression("^[-0-9\\s]{8,30}$", ErrorMessage = "{0}formaterror")]
        public string HomePhone { get; set; }

        [Display(Name = "Address")]
        [StringLength(200, ErrorMessage = "{0}stringmax{1}")]
        public string Address { get; set; }

        [Display(Name = "ZipCode")]
        [RegularExpression("^[0-9]{6,6}$", ErrorMessage = "{0}formaterror")]
        public string ZipCode { get; set; }

        [Display(Name = "Photo")]
        public Guid? PhotoId { get; set; }

        [Display(Name = "Photo")]
        public FileAttachment Photo { get; set; }

        [Display(Name = "IsValid")]
        public bool IsValid { get; set; }

        [Display(Name = "Role")]
        public List<FrameworkUserRole> UserRoles { get; set; }

        [Display(Name = "Group")]
        public List<FrameworkUserGroup> UserGroups { get; set; }

        [Display(Name = "SearchCondition")]
        [JsonIgnore]
        public List<SearchCondition> SearchConditions { get; set; } 

        [NotMapped]
        [Display(Name = "User")]
        public string CodeAndName
        {
            get
            {
                return $"{ITCode}({Name})";
            }
        }
    }
}

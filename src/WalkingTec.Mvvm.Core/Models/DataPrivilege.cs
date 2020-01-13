using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// DataPrivilege
    /// </summary>
    [Table("DataPrivileges")]
    public class DataPrivilege : BasePoco
    {
        [Display(Name = "User")]
        public Guid? UserId { get; set; }
        [Display(Name = "User")]
        [JsonIgnore()]
        public FrameworkUserBase User { get; set; }

        [Display(Name = "Group")]
        public Guid? GroupId { get; set; }
        [Display(Name = "Group")]
        [JsonIgnore()]
        public FrameworkGroup Group { get; set; }

        [Required(ErrorMessage = "{0}required")]
        [StringLength(50,ErrorMessage = "{0}stringmax{1}")]
        [Display(Name = "TableName")]
        public string TableName { get; set; }
        public string RelateId { get; set; }

        [Display(Name = "Domain")]
        public Guid? DomainId { get; set; }

        [Display(Name = "Domain")]
        [JsonIgnore()]
        public FrameworkDomain Domain { get; set; }

    }
}

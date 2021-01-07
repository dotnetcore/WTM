using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// DataPrivilege
    /// </summary>
    [Table("DataPrivileges")]
    public class DataPrivilege : BasePoco
    {
        [Display(Name = "User")]
        public string UserCode { get; set; }

        [Display(Name = "Group")]
        public string GroupCode { get; set; }

        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50,ErrorMessage = "Validate.{0}stringmax{1}")]
        [Display(Name = "TableName")]
        public string TableName { get; set; }
        public string RelateId { get; set; }

        [Display(Name = "Domain")]
        public string Domain { get; set; }

    }
}

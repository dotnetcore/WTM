using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// FrameworkUser
    /// </summary>
    [Table("FrameworkUsers")]
    public class FrameworkUser : FrameworkUserBase
    {

        [Display(Name = "Email")]
        [RegularExpression("^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)+$", ErrorMessage = "{0}formaterror")]
        [StringLength(50, ErrorMessage = "{0}stringmax{1}")]
        public string Email { get; set; }

        [Display(Name = "Gender")]
        public GenderEnum? Gender { get; set; }

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



    }
}

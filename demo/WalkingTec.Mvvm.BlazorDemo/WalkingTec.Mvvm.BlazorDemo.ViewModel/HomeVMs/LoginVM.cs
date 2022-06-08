using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.ViewModel.HomeVMs
{
    public class LoginVM : BaseVM
    {
        [Display(Name = "_Admin.Account")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50)]
        public string ITCode { get; set; }

        [Display(Name = "_Admin.Password")]
        [Required(ErrorMessage = "Validate.{0}required")]
        [StringLength(50, ErrorMessage = "Validate.{0}stringmax{1}")]
        public string Password { get; set; }

        [Display(Name = "_Admin.Tenant")]
        public string Tenant { get; set; }

    }
}

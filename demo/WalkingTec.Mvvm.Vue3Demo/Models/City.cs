using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.ReactDemo.Models
{
    public class City : TreePoco<City>
    {
        [Display(Name = "名称")]
        public string Name { get; set; }

        public int Level { get; set; }

        [NotMapped]
        public string Code { get; set; }

        [NotMapped]
        public string ParentCode { get; set; }

    }
}

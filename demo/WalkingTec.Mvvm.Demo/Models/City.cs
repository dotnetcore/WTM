using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.Models
{
    public class City : TopBasePoco, ITreeData<City>
    {
        [Display(Name = "名称")]
        [Required(ErrorMessage = "名称是必填项")]
        public string Name { get; set; }

        public List<City> Children { get; set; }

        [Display(Name = "父级")]
        public City Parent { get; set; }

        [Display(Name = "父级")]
        public Guid? ParentId { get; set; }

    }
}

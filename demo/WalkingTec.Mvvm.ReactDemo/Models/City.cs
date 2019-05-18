using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.ReactDemo.Models
{
    public class City : TopBasePoco,ITreeData<City>
    {
        [Display(Name = "名称")]
        public string Name { get; set; }

        public List<City> Children { get; set; }

        public City Parent { get; set; }

        public Guid? ParentId { get; set; }

        public int Level { get; set; }

        [NotMapped]
        public string Code { get; set; }

        [NotMapped]
        public string ParentCode { get; set; }

    }
}

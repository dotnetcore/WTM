using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WalkingTec.Mvvm.Core
{
    public abstract class TreePoco : TopBasePoco
    {
        [Display(Name = "_Admin.Parent")]
        public Guid? ParentId { get; set; }

    }

    public class TreePoco<T> : TreePoco where T:TreePoco<T>
    {

        [Display(Name = "_Admin.Parent")]
        [JsonIgnore]
        public T Parent { get; set; }
        [InverseProperty("Parent")]
        public List<T> Children { get; set; }
    }

}

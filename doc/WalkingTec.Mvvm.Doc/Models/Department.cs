using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Doc.Models
{
    public class Department : BasePoco,ITreeData<Department>
    {
        [Display(Name = "DepName")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string DepName { get; set; }

        public List<Department> Children { get; set; }

        [Display(Name = "ParentDep")]
        public Department Parent { get; set; }

        [Display(Name = "ParentDep")]
        public Guid? ParentId { get; set; }
    }
}

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
        [Display(Name = "部门名称")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string DepName { get; set; }

        public List<Department> Children { get; set; }

        [Display(Name = "上级部门")]
        public Department Parent { get; set; }

        [Display(Name = "上级部门")]
        public Guid? ParentId { get; set; }
    }
}

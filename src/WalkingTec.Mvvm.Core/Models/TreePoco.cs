using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 树形结构基类，提供父节点 ID 字段，继承本类的模型可按树形层级组织数据
    /// </summary>
    public  class TreePoco : TopBasePoco
    {
        /// <summary>
        /// 父节点 ID；为空表示当前节点为根节点
        /// </summary>
        [Display(Name = "_Admin.Parent")]
        public Guid? ParentId { get; set; }

    }

    /// <summary>
    /// 泛型树形结构基类，通过泛型自引用方式为具体实体提供强类型的父子导航属性
    /// </summary>
    public class TreePoco<T> : TreePoco where T:TreePoco<T>
    {
        /// <summary>
        /// 父节点的导航属性
        /// </summary>
        [Display(Name = "_Admin.Parent")]
        [JsonIgnore]
        public T Parent { get; set; }
        /// <summary>
        /// 子节点集合的导航属性
        /// </summary>
        [InverseProperty("Parent")]
        [Display(Name = "_Admin.Children")]
        public List<T> Children { get; set; }

        /// <summary>
        /// 是否存在子节点
        /// </summary>
        [NotMapped]
        public bool HasChildren
        {
            get
            {
                return Children?.Any() == true;
            }
        }
    }

}

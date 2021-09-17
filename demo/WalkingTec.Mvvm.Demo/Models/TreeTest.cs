using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.Models
{
    [Table("TreeTests")]
    public class TreeTest : TreePoco<TreeTest>, IBasePoco,IPersistPoco
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new int ID { get; set; }

        public new int? ParentId { get; set; }

        [Display(Name = "名称")]
        [Required(ErrorMessage = "名称是必填项")]
        [StringLength(3,ErrorMessage = "{0}abc{1}")]
        public string Name { get; set; }

        [Display(Name = "test")]
        [JsonConverter(typeof(JsonStringConverter))]
        public string Test { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateBy { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string UpdateBy { get; set; }
        public bool IsValid { get; set; } = true;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WalkingTec.Mvvm.Core.Test
{
    [Display(Name = "货物种类")]
    [Table("zz_goods_catalog")]
    public class GoodsCatalog : PersistPoco
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new int ID { get; set; }

        [Display(Name = "种类名称")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        [Required(ErrorMessage = "{0}是必填项")]
        public string Name { get; set; }

        [Display(Name = "序号")]
        public int? OrderNum { get; set; }

        [Display(Name = "备注")]
        [StringLength(200, ErrorMessage = "{0}最多输入{1}个字符")]
        public string Remark { get; set; }
    }
}

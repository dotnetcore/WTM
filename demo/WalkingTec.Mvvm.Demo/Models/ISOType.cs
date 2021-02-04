using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.Models
{
    public class ISOType : BasePoco
    {
        [Display(Name = "ISO名称")]
        public string IsoName { get; set; }
        [Display(Name = " 版本号")]
        [Required(ErrorMessage = "{0}是必填项")]
        [StringLength(50, ErrorMessage = "{0}最多输入{1}个字符")]
        public string ISOVerSion { get; set; }
        [Display(Name = " 附加信息")]
        public string Description { get; set; }
        [Display(Name = "ISO")]
        public Guid? ISOFileID { get; set; }
        public FileAttachment ISOFile { get; set; }

        [Display(Name = "EXE版本")]
        public List<ISOEXE> iSOTypes { get; set; }
    }
}

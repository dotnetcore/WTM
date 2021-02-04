using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Attributes;

namespace WalkingTec.Mvvm.Demo.Models
{
    [MiddleTable]
    public class ISOEXE : BasePoco
    {
        public SoftFacInfo softFacInfo { get; set; }
        public ISOType isoType { get; set; }

        [Display(Name = "EXEID")]
        public Guid softFacInfoID { get; set; }

        [Display(Name = "ISOID")]
        public Guid isoTypeID { get; set; }
    }
}

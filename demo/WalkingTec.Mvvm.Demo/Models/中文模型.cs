using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Demo.Models
{
    public class 不要用中文模型名 : TopBasePoco
    {
        public string 不要 { get; set; }
        public int 用 { get; set; }

        public DateTime? 中文 { get; set; }

        public bool 模型名 { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.不要用中文模型名VMs
{
    public partial class 不要用中文模型名Searcher : BaseSearcher
    {
        public String 不要 { get; set; }
        public Int32? 用 { get; set; }
        public DateTime? 中文 { get; set; }
        public Boolean? 模型名 { get; set; }

        protected override void InitVM()
        {
        }

    }
}

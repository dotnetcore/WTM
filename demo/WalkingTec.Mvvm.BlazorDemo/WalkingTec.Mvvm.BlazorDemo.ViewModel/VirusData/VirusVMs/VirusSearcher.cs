using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models.Virus;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.VirusData.VirusVMs
{
    public partial class VirusSearcher : BaseSearcher
    {
        [Display(Name = "病毒名称")]
        public String VirtusName { get; set; }
        [Display(Name = "病毒代码")]
        public String VirtusCode { get; set; }
        [Display(Name = "病毒描述")]
        public String Remark { get; set; }
        [Display(Name = "病毒种类")]
        public VirtusTypeEnum? VirtusType { get; set; }
        [Display(Name = "患者")]
        public List<int> SelectedPatientsIDs { get; set; }

        protected override void InitVM()
        {
        }

    }
}

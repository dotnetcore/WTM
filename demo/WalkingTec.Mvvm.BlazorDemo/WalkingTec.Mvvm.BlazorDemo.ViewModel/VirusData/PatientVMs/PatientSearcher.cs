using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models.Virus;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.VirusData.PatientVMs
{
    public partial class PatientSearcher : BaseSearcher
    {
        [Display(Name = "患者姓名")]
        public String PatientName { get; set; }
        [Display(Name = "身份证")]
        public String IdNumber { get; set; }
        [Display(Name = "性别")]
        public GenderEnum? Gender { get; set; }
        [Display(Name = "状态")]
        public PatientStatusEnum? Status { get; set; }
        [Display(Name = "生日")]
        public DateRange Birthday { get; set; }
        [Display(Name = "籍贯")]
        public int? LocationId { get; set; }
        [Display(Name = "所属医院")]
        public Guid? HospitalId { get; set; }
        [Display(Name = "病毒")]
        public List<Guid> SelectedVirusesIDs { get; set; }

        protected override async Task InitVM()
        {
        }

    }
}

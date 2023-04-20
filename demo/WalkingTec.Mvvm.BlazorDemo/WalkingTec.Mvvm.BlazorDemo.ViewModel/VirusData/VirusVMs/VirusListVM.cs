using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models.Virus;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.VirusData.VirusVMs
{
    public partial class VirusListVM : BasePagedListVM<Virus_View, VirusSearcher>
    {

        protected override Task<IEnumerable<IGridColumn<Virus_View>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<Virus_View>>> (new List<GridColumn<Virus_View>>{
                this.MakeGridHeader(x => x.VirtusName),
                this.MakeGridHeader(x => x.VirtusCode),
                this.MakeGridHeader(x => x.Remark),
                this.MakeGridHeader(x => x.VirtusType),
                this.MakeGridHeader(x => x.PatientName_view),
                this.MakeGridHeaderAction(width: 200)
            });
        }

        public override Task<IOrderedQueryable<Virus_View>> GetSearchQuery()
        {
            var query = DC.Set<Virus>()
                .CheckContain(Searcher.VirtusName, x=>x.VirtusName)
                .CheckContain(Searcher.VirtusCode, x=>x.VirtusCode)
                .CheckEqual(Searcher.VirtusType, x=>x.VirtusType)
                .CheckWhere(Searcher.SelectedPatientsIDs,x=>DC.Set<PatientVirus>().Where(y=>Searcher.SelectedPatientsIDs.Contains(y.PatientId)).Select(z=>z.VirusId).Contains(x.ID))
                .Select(x => new Virus_View
                {
				    ID = x.ID,
                    VirtusName = x.VirtusName,
                    VirtusCode = x.VirtusCode,
                    Remark = x.Remark,
                    VirtusType = x.VirtusType,
                    PatientName_view = x.Patients.Where(y=>y.Patient.IsValid==true).Select(y=>y.Patient.PatientName).ToSepratedString(null,","), 
                })
                .OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

    }

    public class Virus_View : Virus{
        [Display(Name = "患者姓名")]
        public String PatientName_view { get; set; }

    }
}

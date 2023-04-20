using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models.Virus;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.BlazorDemo.ViewModel.VirusData.PatientVMs
{
    public partial class PatientListVM : BasePagedListVM<Patient_View, PatientSearcher>
    {

        protected override Task<IEnumerable<IGridColumn<Patient_View>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<Patient_View>>> (new List<GridColumn<Patient_View>>{
                this.MakeGridHeader(x => x.PatientName),
                this.MakeGridHeader(x => x.IdNumber),
                this.MakeGridHeader(x => x.Gender),
                this.MakeGridHeader(x => x.Status),
                this.MakeGridHeader(x => x.Birthday),
                this.MakeGridHeader(x => x.Name_view),
                this.MakeGridHeader(x => x.Name_view2),
                this.MakeGridHeader(x => x.PhotoId).SetFormat(PhotoIdFormat),
                this.MakeGridHeader(x => x.VirtusName_view),
                this.MakeGridHeaderAction(width: 200)
            });
        }
        private List<ColumnFormatInfo> PhotoIdFormat(Patient_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.PhotoId),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.PhotoId,640,480),
            };
        }


        public override Task<IOrderedQueryable<Patient_View>> GetSearchQuery()
        {
            var query = DC.Set<Patient>()
                .CheckContain(Searcher.PatientName, x=>x.PatientName)
                .CheckContain(Searcher.IdNumber, x=>x.IdNumber)
                .CheckEqual(Searcher.Gender, x=>x.Gender)
                .CheckEqual(Searcher.Status, x=>x.Status)
                .CheckBetween(Searcher.Birthday?.GetStartTime(), Searcher.Birthday?.GetEndTime(), x => x.Birthday, includeMax: false)
                .CheckEqual(Searcher.LocationId, x=>x.LocationId)
                .CheckEqual(Searcher.HospitalId, x=>x.HospitalId)
                .CheckWhere(Searcher.SelectedVirusesIDs,x=>DC.Set<PatientVirus>().Where(y=>Searcher.SelectedVirusesIDs.Contains(y.VirusId)).Select(z=>z.PatientId).Contains(x.ID))
                .Select(x => new Patient_View
                {
				    ID = x.ID,
                    PatientName = x.PatientName,
                    IdNumber = x.IdNumber,
                    Gender = x.Gender,
                    Status = x.Status,
                    Birthday = x.Birthday,
                    Name_view = x.Location.Name,
                    Name_view2 = x.Hospital.Name,
                    PhotoId = x.PhotoId,
                    VirtusName_view = x.Viruses.Select(y=>y.Virus.VirtusName).ToSepratedString(null,","), 
                })
                .OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

    }

    public class Patient_View : Patient{
        [Display(Name = "名称")]
        public String Name_view { get; set; }
        [Display(Name = "医院名称")]
        public String Name_view2 { get; set; }
        [Display(Name = "病毒名称")]
        public String VirtusName_view { get; set; }

    }
}

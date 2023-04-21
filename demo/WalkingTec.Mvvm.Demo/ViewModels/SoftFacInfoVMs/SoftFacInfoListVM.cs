using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.SoftFacInfoVMs
{
    public partial class SoftFacInfoListVM : BasePagedListVM<SoftFacInfo_View, SoftFacInfoSearcher>
    {
        protected override Task<List<GridAction>> InitGridAction()
        {
            return Task.FromResult (new List<GridAction>
            {
                this.MakeStandardAction("SoftFacInfo", GridActionStandardTypesEnum.Create, Localizer["Sys.Create"],"", dialogWidth: 800),
                this.MakeStandardAction("SoftFacInfo", GridActionStandardTypesEnum.Edit, Localizer["Sys.Edit"], "", dialogWidth: 800),
                this.MakeStandardAction("SoftFacInfo", GridActionStandardTypesEnum.Delete, Localizer["Sys.Delete"], "", dialogWidth: 800),
                this.MakeStandardAction("SoftFacInfo", GridActionStandardTypesEnum.Details, Localizer["Sys.Details"], "", dialogWidth: 800),
                this.MakeStandardAction("SoftFacInfo", GridActionStandardTypesEnum.BatchEdit, Localizer["Sys.BatchEdit"], "", dialogWidth: 800),
                this.MakeStandardAction("SoftFacInfo", GridActionStandardTypesEnum.BatchDelete, Localizer["Sys.BatchDelete"], "", dialogWidth: 800),
                this.MakeStandardAction("SoftFacInfo", GridActionStandardTypesEnum.Import, Localizer["Sys.Import"], "", dialogWidth: 800),
                this.MakeStandardAction("SoftFacInfo", GridActionStandardTypesEnum.ExportExcel, Localizer["Sys.Export"], ""),
            });
        }


        protected override Task<IEnumerable<IGridColumn<SoftFacInfo_View>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<SoftFacInfo_View>>> (new List<GridColumn<SoftFacInfo_View>>{
                this.MakeGridHeader(x => x.IsoName),
                this.MakeGridHeader(x => x.EXEVerSion),
                this.MakeGridHeader(x => x.Description),
                this.MakeGridHeader(x => x.EXEFileID).SetFormat(EXEFileIDFormat),
                this.MakeGridHeader(x => x.IsoName_view),
                this.MakeGridHeaderAction(width: 200)
            });
        }
        private List<ColumnFormatInfo> EXEFileIDFormat(SoftFacInfo_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.EXEFileID),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.EXEFileID,640,480),
            };
        }


        public override Task<IOrderedQueryable<SoftFacInfo_View>> GetSearchQuery()
        {
            var query = DC.Set<SoftFacInfo>()
                .CheckContain(Searcher.IsoName, x=>x.IsoName)
                .CheckContain(Searcher.EXEVerSion, x=>x.EXEVerSion)
                .CheckContain(Searcher.Description, x=>x.Description)
                .CheckWhere(Searcher.SelectediSOTypesIDs,x=>DC.Set<ISOEXE>().Where(y=>Searcher.SelectediSOTypesIDs.Contains(y.isoTypeID)).Select(z=>z.softFacInfoID).Contains(x.ID))
                .Select(x => new SoftFacInfo_View
                {
				    ID = x.ID,
                    IsoName = x.IsoName,
                    EXEVerSion = x.EXEVerSion,
                    Description = x.Description,
                    EXEFileID = x.EXEFileID,
                    IsoName_view = x.iSOTypes.Select(y=>y.isoType.IsoName).ToSepratedString(null,","), 
                })
                .OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

    }

    public class SoftFacInfo_View : SoftFacInfo{
        [Display(Name = "ISO名称")]
        public String IsoName_view { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.ISOTypeVMs
{
    public partial class ISOTypeListVM : BasePagedListVM<ISOType_View, ISOTypeSearcher>
    {
        protected override Task<List<GridAction>> InitGridAction()
        {
            return Task.FromResult (new List<GridAction>
            {
                this.MakeStandardAction("ISOType", GridActionStandardTypesEnum.Create, Localizer["Sys.Create"],"", dialogWidth: 800),
                this.MakeStandardAction("ISOType", GridActionStandardTypesEnum.Edit, Localizer["Sys.Edit"], "", dialogWidth: 800),
                this.MakeStandardAction("ISOType", GridActionStandardTypesEnum.Delete, Localizer["Sys.Delete"], "", dialogWidth: 800),
                this.MakeStandardAction("ISOType", GridActionStandardTypesEnum.Details, Localizer["Sys.Details"], "", dialogWidth: 800),
                this.MakeStandardAction("ISOType", GridActionStandardTypesEnum.BatchEdit, Localizer["Sys.BatchEdit"], "", dialogWidth: 800),
                this.MakeStandardAction("ISOType", GridActionStandardTypesEnum.BatchDelete, Localizer["Sys.BatchDelete"], "", dialogWidth: 800),
                this.MakeStandardAction("ISOType", GridActionStandardTypesEnum.Import, Localizer["Sys.Import"], "", dialogWidth: 800),
                this.MakeStandardAction("ISOType", GridActionStandardTypesEnum.ExportExcel, Localizer["Sys.Export"], ""),
            });
        }


        protected override Task<IEnumerable<IGridColumn<ISOType_View>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<ISOType_View>>> (new List<GridColumn<ISOType_View>>{
                this.MakeGridHeader(x => x.IsoName),
                this.MakeGridHeader(x => x.ISOVerSion),
                this.MakeGridHeader(x => x.Description),
                this.MakeGridHeader(x => x.ISOFileID).SetFormat(ISOFileIDFormat),
                this.MakeGridHeader(x => x.IsoName_view),
                this.MakeGridHeaderAction(width: 200)
            });
        }
        private List<ColumnFormatInfo> ISOFileIDFormat(ISOType_View entity, object val)
        {
            return new List<ColumnFormatInfo>
            {
                ColumnFormatInfo.MakeDownloadButton(ButtonTypesEnum.Button,entity.ISOFileID),
                ColumnFormatInfo.MakeViewButton(ButtonTypesEnum.Button,entity.ISOFileID,640,480),
            };
        }


        public override Task<IOrderedQueryable<ISOType_View>> GetSearchQuery()
        {
            var query = DC.Set<ISOType>()
                .CheckContain(Searcher.IsoName, x=>x.IsoName)
                .CheckContain(Searcher.ISOVerSion, x=>x.ISOVerSion)
                .CheckContain(Searcher.Description, x=>x.Description)
                .CheckWhere(Searcher.SelectediSOTypesIDs,x=>DC.Set<ISOEXE>().Where(y=>Searcher.SelectediSOTypesIDs.Contains(y.softFacInfoID)).Select(z=>z.isoTypeID).Contains(x.ID))
                .Select(x => new ISOType_View
                {
				    ID = x.ID,
                    IsoName = x.IsoName,
                    ISOVerSion = x.ISOVerSion,
                    Description = x.Description,
                    ISOFileID = x.ISOFileID,
                    IsoName_view = x.iSOTypes.Select(y=>y.softFacInfo.IsoName).ToSepratedString(null,","), 
                })
                .OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

    }

    public class ISOType_View : ISOType{
        [Display(Name = "exe名称")]
        public String IsoName_view { get; set; }

    }
}

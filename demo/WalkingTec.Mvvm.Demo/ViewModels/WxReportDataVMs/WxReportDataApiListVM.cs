using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.WxReportDataVMs
{
    public partial class WxReportDataApiListVM : BasePagedListVM<WxReportDataApi_View, WxReportDataApiSearcher>
    {

        protected override Task<IEnumerable<IGridColumn<WxReportDataApi_View>>> InitGridHeader()
        {
            return Task.FromResult<IEnumerable<IGridColumn<WxReportDataApi_View>>> (new List<GridColumn<WxReportDataApi_View>>{
                this.MakeGridHeader(x => x.ToWxUser),
                this.MakeGridHeader(x => x.Name_view),
                this.MakeGridHeader(x => x.Date),
                this.MakeGridHeader(x => x.Type),
                this.MakeGridHeader(x => x.DataType),
                this.MakeGridHeader(x => x.TuFang1),
                this.MakeGridHeader(x => x.KM1),
                this.MakeGridHeader(x => x.TuFang2),
                this.MakeGridHeader(x => x.KM2),
                this.MakeGridHeader(x => x.TuFang3),
                this.MakeGridHeader(x => x.KM3),
                this.MakeGridHeader(x => x.TuFang4),
                this.MakeGridHeader(x => x.KM4),
                this.MakeGridHeader(x => x.TuFang5),
                this.MakeGridHeader(x => x.KM5),
                this.MakeGridHeader(x => x.TuFang6),
                this.MakeGridHeader(x => x.KM6),
                this.MakeGridHeader(x => x.YnNi1),
                this.MakeGridHeader(x => x.YnNi1KM),
                this.MakeGridHeader(x => x.YnNi2),
                this.MakeGridHeader(x => x.YnNi2KM),
                this.MakeGridHeader(x => x.YnNi3),
                this.MakeGridHeader(x => x.YnNi3KM),
                this.MakeGridHeader(x => x.YnNi4),
                this.MakeGridHeader(x => x.YnNi4KM),
                this.MakeGridHeader(x => x.JiaYou1),
                this.MakeGridHeader(x => x.JiaYou2),
                this.MakeGridHeader(x => x.CanFeiRen),
                this.MakeGridHeader(x => x.JieZhiMoney),
                this.MakeGridHeader(x => x.Extend01),
                this.MakeGridHeader(x => x.Extend02),
                this.MakeGridHeader(x => x.Extend03),
                this.MakeGridHeader(x => x.Extend04),
                this.MakeGridHeader(x => x.Extend05),
                this.MakeGridHeader(x => x.Extend07),
                this.MakeGridHeader(x => x.Extend08),
                this.MakeGridHeader(x => x.Extend09),
                this.MakeGridHeader(x => x.Extend10),
                this.MakeGridHeaderAction(width: 200)
            });
        }

        public override Task<IOrderedQueryable<WxReportDataApi_View>> GetSearchQuery()
        {
            var query = DC.Set<WxReportData>()
                .CheckEqual(Searcher.FrameworkUserId, x=>x.FrameworkUserId)
                .CheckEqual(Searcher.JiaYou1, x=>x.JiaYou1)
                .CheckEqual(Searcher.JiaYou2, x=>x.JiaYou2)
                .CheckEqual(Searcher.Extend09, x=>x.Extend09)
                .CheckEqual(Searcher.Extend10, x=>x.Extend10)
                .Select(x => new WxReportDataApi_View
                {
				    ID = x.ID,
                    ToWxUser = x.ToWxUser,
                    Name_view = x.FrameworkUser.Name,
                    Date = x.Date,
                    Type = x.Type,
                    DataType = x.DataType,
                    TuFang1 = x.TuFang1,
                    KM1 = x.KM1,
                    TuFang2 = x.TuFang2,
                    KM2 = x.KM2,
                    TuFang3 = x.TuFang3,
                    KM3 = x.KM3,
                    TuFang4 = x.TuFang4,
                    KM4 = x.KM4,
                    TuFang5 = x.TuFang5,
                    KM5 = x.KM5,
                    TuFang6 = x.TuFang6,
                    KM6 = x.KM6,
                    YnNi1 = x.YnNi1,
                    YnNi1KM = x.YnNi1KM,
                    YnNi2 = x.YnNi2,
                    YnNi2KM = x.YnNi2KM,
                    YnNi3 = x.YnNi3,
                    YnNi3KM = x.YnNi3KM,
                    YnNi4 = x.YnNi4,
                    YnNi4KM = x.YnNi4KM,
                    JiaYou1 = x.JiaYou1,
                    JiaYou2 = x.JiaYou2,
                    CanFeiRen = x.CanFeiRen,
                    JieZhiMoney = x.JieZhiMoney,
                    Extend01 = x.Extend01,
                    Extend02 = x.Extend02,
                    Extend03 = x.Extend03,
                    Extend04 = x.Extend04,
                    Extend05 = x.Extend05,
                    Extend07 = x.Extend07,
                    Extend08 = x.Extend08,
                    Extend09 = x.Extend09,
                    Extend10 = x.Extend10,
                })
                .OrderBy(x => x.ID);
            return Task.FromResult (query);
        }

    }

    public class WxReportDataApi_View : WxReportData{
        [Display(Name = "_Admin.Name")]
        public String Name_view { get; set; }

    }
}

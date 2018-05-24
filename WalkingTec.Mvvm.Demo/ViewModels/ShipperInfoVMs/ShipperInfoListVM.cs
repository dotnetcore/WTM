using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Demo;


namespace WalkingTec.Mvvm.Demo.ViewModels.ShipperInfoVMs
{
    public class ShipperInfoListVM : BasePagedListVM<ShipperInfo_View, ShipperInfoSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("ShipperInfo", GridActionStandardTypesEnum.Create, "新建","", dialogWidth: 800),
                this.MakeStandardAction("ShipperInfo", GridActionStandardTypesEnum.Edit, "修改","", dialogWidth: 800),
                this.MakeStandardAction("ShipperInfo", GridActionStandardTypesEnum.Delete, "删除", "",dialogWidth: 800),
                this.MakeStandardAction("ShipperInfo", GridActionStandardTypesEnum.Details, "详细","", dialogWidth: 800),
                this.MakeStandardAction("ShipperInfo", GridActionStandardTypesEnum.BatchEdit, "批量修改","", dialogWidth: 800),
                this.MakeStandardAction("ShipperInfo", GridActionStandardTypesEnum.BatchDelete, "批量删除","", dialogWidth: 800),
                this.MakeStandardAction("ShipperInfo", GridActionStandardTypesEnum.Import, "导入","", dialogWidth: 800),
                this.MakeStandardExportAction(null,false,ExportEnum.Excel)
            };
        }

        protected override IEnumerable<IGridColumn<ShipperInfo_View>> InitGridHeader()
        {
            return new List<GridColumn<ShipperInfo_View>>{
                this.MakeGridHeader(x => x.shipper_code),
                this.MakeGridHeader(x => x.shipper_name),
                this.MakeGridHeader(x => x.shipper_company),
                this.MakeGridHeader(x => x.shipper_countrycode),
                this.MakeGridHeader(x => x.shipper_province),
                this.MakeGridHeader(x => x.shipper_city),
                this.MakeGridHeader(x => x.shipper_street),
                this.MakeGridHeader(x => x.shipper_postcode),
                this.MakeGridHeader(x => x.shipper_areacode),
                this.MakeGridHeader(x => x.shipper_telephone),
                this.MakeGridHeader(x => x.shipper_mobile),
                this.MakeGridHeader(x => x.shipper_email),
                this.MakeGridHeader(x => x.shipper_fax),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<ShipperInfo_View> GetSearchQuery()
        {
            var query = DC.Set<ShipperInfo>()
                .CheckContain(Searcher.shipper_code, x=>x.shipper_code)
                .CheckContain(Searcher.shipper_name, x=>x.shipper_name)
                .Select(x => new ShipperInfo_View
                {
				    ID = x.ID,
                    shipper_code = x.shipper_code,
                    shipper_name = x.shipper_name,
                    shipper_company = x.shipper_company,
                    shipper_countrycode = x.shipper_countrycode,
                    shipper_province = x.shipper_province,
                    shipper_city = x.shipper_city,
                    shipper_street = x.shipper_street,
                    shipper_postcode = x.shipper_postcode,
                    shipper_areacode = x.shipper_areacode,
                    shipper_telephone = x.shipper_telephone,
                    shipper_mobile = x.shipper_mobile,
                    shipper_email = x.shipper_email,
                    shipper_fax = x.shipper_fax,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class ShipperInfo_View : ShipperInfo{

    }
}

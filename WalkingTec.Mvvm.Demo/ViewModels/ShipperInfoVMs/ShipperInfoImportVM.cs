using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo;


namespace WalkingTec.Mvvm.Demo.ViewModels.ShipperInfoVMs
{
    public class ShipperInfoTemplateVM : BaseTemplateVM
    {
        [Display(Name = "发件人code")]
        public ExcelPropety shipper_code_Excel = ExcelPropety.CreateProperty<ShipperInfo>(x => x.shipper_code);
        [Display(Name = "发件人姓名")]
        public ExcelPropety shipper_name_Excel = ExcelPropety.CreateProperty<ShipperInfo>(x => x.shipper_name);
        [Display(Name = "发件人公司名")]
        public ExcelPropety shipper_company_Excel = ExcelPropety.CreateProperty<ShipperInfo>(x => x.shipper_company);
        [Display(Name = "发件人国家代码")]
        public ExcelPropety shipper_countrycode_Excel = ExcelPropety.CreateProperty<ShipperInfo>(x => x.shipper_countrycode);
        [Display(Name = "发件人省")]
        public ExcelPropety shipper_province_Excel = ExcelPropety.CreateProperty<ShipperInfo>(x => x.shipper_province);
        [Display(Name = "发件人城市")]
        public ExcelPropety shipper_city_Excel = ExcelPropety.CreateProperty<ShipperInfo>(x => x.shipper_city);
        [Display(Name = "发件人地址")]
        public ExcelPropety shipper_street_Excel = ExcelPropety.CreateProperty<ShipperInfo>(x => x.shipper_street);
        [Display(Name = "发件人邮编")]
        public ExcelPropety shipper_postcode_Excel = ExcelPropety.CreateProperty<ShipperInfo>(x => x.shipper_postcode);
        [Display(Name = "发件人区域代码")]
        public ExcelPropety shipper_areacode_Excel = ExcelPropety.CreateProperty<ShipperInfo>(x => x.shipper_areacode);
        [Display(Name = "发件人电话")]
        public ExcelPropety shipper_telephone_Excel = ExcelPropety.CreateProperty<ShipperInfo>(x => x.shipper_telephone);
        [Display(Name = "发件人手机")]
        public ExcelPropety shipper_mobile_Excel = ExcelPropety.CreateProperty<ShipperInfo>(x => x.shipper_mobile);
        [Display(Name = "发件人邮箱")]
        public ExcelPropety shipper_email_Excel = ExcelPropety.CreateProperty<ShipperInfo>(x => x.shipper_email);
        [Display(Name = "发件人传真")]
        public ExcelPropety shipper_fax_Excel = ExcelPropety.CreateProperty<ShipperInfo>(x => x.shipper_fax);

	    protected override void InitVM()
        {
        }

    }

    public class ShipperInfoImportVM : BaseImportVM<ShipperInfoTemplateVM, ShipperInfo>
    {

    }

}

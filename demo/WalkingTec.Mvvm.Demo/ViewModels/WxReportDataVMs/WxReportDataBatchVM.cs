using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.WxReportDataVMs
{
    public partial class WxReportDataBatchVM : BaseBatchVM<WxReportData, WxReportData_BatchEdit>
    {
        public WxReportDataBatchVM()
        {
            ListVM = new WxReportDataListVM();
            LinkedVM = new WxReportData_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class WxReportData_BatchEdit : BaseVM
    {
        [Display(Name = "扩展字段")]
        public String Extend04 { get; set; }
        [Display(Name = "扩展字段")]
        public String Extend05 { get; set; }
        [Display(Name = "扩展字段")]
        public Int32? Extend07 { get; set; }
        [Display(Name = "扩展字段")]
        public Int32? Extend08 { get; set; }
        [Display(Name = "扩展字段")]
        public Double? Extend09 { get; set; }

        protected override void InitVM()
        {
        }

    }

}

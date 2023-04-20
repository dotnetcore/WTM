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
    public partial class WxReportDataApiBatchVM : BaseBatchVM<WxReportData, WxReportDataApi_BatchEdit>
    {
        public WxReportDataApiBatchVM()
        {
            ListVM = new WxReportDataApiListVM();
            LinkedVM = new WxReportDataApi_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class WxReportDataApi_BatchEdit : BaseVM
    {
        [Display(Name = "姓名")]
        public Guid? FrameworkUserId { get; set; }
        [Display(Name = "加油1:升")]
        public Double? JiaYou1 { get; set; }
        [Display(Name = "加油2:升")]
        public Double? JiaYou2 { get; set; }
        [Display(Name = "扩展字段")]
        public Double? Extend09 { get; set; }
        [Display(Name = "扩展字段")]
        public Double? Extend10 { get; set; }

        protected override async Task InitVM()
        {
        }

    }

}

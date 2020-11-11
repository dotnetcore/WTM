using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.MyUserVMs
{
    public partial class MyUserTemplateVM : BaseTemplateVM
    {
        [Display(Name = "附加信息1")]
        public ExcelPropety Extra1_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.Extra1);
        [Display(Name = "附加信息2")]
        public ExcelPropety Extra2_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.Extra2);
        [Display(Name = "Account")]
        public ExcelPropety ITCode_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.ITCode);
        [Display(Name = "Password")]
        public ExcelPropety Password_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.Password);
        [Display(Name = "Name")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.Name);
        [Display(Name = "IsValid")]
        public ExcelPropety IsValid_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.IsValid);

	    protected override void InitVM()
        {
        }

    }

    public class MyUserImportVM : BaseImportVM<MyUserTemplateVM, MyUser>
    {

    }

}

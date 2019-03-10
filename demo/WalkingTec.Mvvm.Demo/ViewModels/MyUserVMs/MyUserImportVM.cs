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
    public class MyUserTemplateVM : BaseTemplateVM
    {
        [Display(Name = "附加信息1")]
        public ExcelPropety Extra1_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.Extra1);
        [Display(Name = "附加信息2")]
        public ExcelPropety Extra2_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.Extra2);
        [Display(Name = "账号")]
        public ExcelPropety ITCode_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.ITCode);
        [Display(Name = "密码")]
        public ExcelPropety Password_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.Password);
        [Display(Name = "邮箱")]
        public ExcelPropety Email_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.Email);
        [Display(Name = "姓名")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.Name);
        [Display(Name = "性别")]
        public ExcelPropety Sex_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.Sex);
        [Display(Name = "手机")]
        public ExcelPropety CellPhone_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.CellPhone);
        [Display(Name = "座机")]
        public ExcelPropety HomePhone_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.HomePhone);
        [Display(Name = "住址")]
        public ExcelPropety Address_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.Address);
        [Display(Name = "邮编")]
        public ExcelPropety ZipCode_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.ZipCode);
        [Display(Name = "是否有效")]
        public ExcelPropety IsValid_Excel = ExcelPropety.CreateProperty<MyUser>(x => x.IsValid);

	    protected override void InitVM()
        {
        }

    }

    public class MyUserImportVM : BaseImportVM<MyUserTemplateVM, MyUser>
    {

    }

}

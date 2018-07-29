using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.StudentVMs
{
    public class StudentTemplateVM : BaseTemplateVM
    {
        [Display(Name = "账号")]
        public ExcelPropety LoginName_Excel = ExcelPropety.CreateProperty<Student>(x => x.LoginName);
        [Display(Name = "密码")]
        public ExcelPropety Password_Excel = ExcelPropety.CreateProperty<Student>(x => x.Password);
        [Display(Name = "邮箱")]
        public ExcelPropety Email_Excel = ExcelPropety.CreateProperty<Student>(x => x.Email);
        [Display(Name = "姓名")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<Student>(x => x.Name);
        [Display(Name = "性别")]
        public ExcelPropety Sex_Excel = ExcelPropety.CreateProperty<Student>(x => x.Sex);
        [Display(Name = "手机")]
        public ExcelPropety CellPhone_Excel = ExcelPropety.CreateProperty<Student>(x => x.CellPhone);
        [Display(Name = "住址")]
        public ExcelPropety Address_Excel = ExcelPropety.CreateProperty<Student>(x => x.Address);
        [Display(Name = "邮编")]
        public ExcelPropety ZipCode_Excel = ExcelPropety.CreateProperty<Student>(x => x.ZipCode);
        [Display(Name = "是否有效")]
        public ExcelPropety IsValid_Excel = ExcelPropety.CreateProperty<Student>(x => x.IsValid);
        [Display(Name = "日期")]
        public ExcelPropety EnRollDate_Excel = ExcelPropety.CreateProperty<Student>(x => x.EnRollDate);

	    protected override void InitVM()
        {
        }

    }

    public class StudentImportVM : BaseImportVM<StudentTemplateVM, Student>
    {

    }

}

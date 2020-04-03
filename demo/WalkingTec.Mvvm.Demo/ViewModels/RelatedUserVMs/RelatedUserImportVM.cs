using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.RelatedUserVMs
{
    public partial class RelatedUserTemplateVM : BaseTemplateVM
    {
        [Display(Name = "附加信息1")]
        public ExcelPropety Extra1_Excel = ExcelPropety.CreateProperty<RelatedUser>(x => x.Extra1);
        [Display(Name = "附加信息2")]
        public ExcelPropety Extra2_Excel = ExcelPropety.CreateProperty<RelatedUser>(x => x.Extra2);
        [Display(Name = "Account")]
        public ExcelPropety ITCode_Excel = ExcelPropety.CreateProperty<RelatedUser>(x => x.ITCode);
        [Display(Name = "Password")]
        public ExcelPropety Password_Excel = ExcelPropety.CreateProperty<RelatedUser>(x => x.Password);
        [Display(Name = "Email")]
        public ExcelPropety Email_Excel = ExcelPropety.CreateProperty<RelatedUser>(x => x.Email);
        [Display(Name = "Name")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<RelatedUser>(x => x.Name);
        [Display(Name = "Sex")]
        public ExcelPropety Sex_Excel = ExcelPropety.CreateProperty<RelatedUser>(x => x.Sex);
        [Display(Name = "CellPhone")]
        public ExcelPropety CellPhone_Excel = ExcelPropety.CreateProperty<RelatedUser>(x => x.CellPhone);
        [Display(Name = "HomePhone")]
        public ExcelPropety HomePhone_Excel = ExcelPropety.CreateProperty<RelatedUser>(x => x.HomePhone);
        [Display(Name = "Address")]
        public ExcelPropety Address_Excel = ExcelPropety.CreateProperty<RelatedUser>(x => x.Address);
        [Display(Name = "ZipCode")]
        public ExcelPropety ZipCode_Excel = ExcelPropety.CreateProperty<RelatedUser>(x => x.ZipCode);
        [Display(Name = "IsValid")]
        public ExcelPropety IsValid_Excel = ExcelPropety.CreateProperty<RelatedUser>(x => x.IsValid);

	    protected override void InitVM()
        {
        }

    }

    public class RelatedUserImportVM : BaseImportVM<RelatedUserTemplateVM, RelatedUser>
    {

    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.SchoolVMs
{
    public class SchoolTemplateVM : BaseTemplateVM
    {
        [Display(Name = "学校编码")]
        public ExcelPropety SchoolCode_Excel = ExcelPropety.CreateProperty<School>(x => x.SchoolCode);

        [Display(Name = "学校类型")]
        public ExcelPropety SchoolType_Excel = ExcelPropety.CreateProperty<School>(x => x.SchoolType);

        [Display(Name = "学校名称")]
        public ExcelPropety SchoolName_Excel = ExcelPropety.CreateProperty<School>(x => x.SchoolName);

        [Display(Name = "添加日期")]
        public ExcelPropety SchoolTime_Excel = ExcelPropety.CreateProperty<School>(x => x.CreateTime, true);

        [Display(Name = "班级数量")]
        public ExcelPropety ClassNumber_Excel = ExcelPropety.CreateProperty<School>(x => x.ClassNumber);

        [Display(Name = "基础费用")]
        public ExcelPropety SchoolMoney_Excel = ExcelPropety.CreateProperty<School>(x => x.SchoolMoney);

        [Display(Name = "是否高级")]
        public ExcelPropety IsHgih_Excel = ExcelPropety.CreateProperty<School>(x => x.IsHgih);

        [Display(Name = "备注")]
        public ExcelPropety Remark_Excel = ExcelPropety.CreateProperty<School>(x => x.Remark);

        protected override void InitVM()
        {
        }

    }

    public class SchoolImportVM : BaseImportVM<SchoolTemplateVM, School>
    {

    }

}

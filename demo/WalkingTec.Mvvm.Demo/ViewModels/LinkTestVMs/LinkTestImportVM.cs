using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.LinkTestVMs
{
    public partial class LinkTestTemplateVM : BaseTemplateVM
    {
        public ExcelPropety name_Excel = ExcelPropety.CreateProperty<LinkTest>(x => x.name);
        public ExcelPropety Student_Excel = ExcelPropety.CreateProperty<LinkTest>(x => x.StudentId);

	    protected override void InitVM()
        {
            Student_Excel.DataType = ColumnDataType.ComboBox;
            Student_Excel.ListItems = DC.Set<Student>().GetSelectListItems(Wtm, y => y.Name);
        }

    }

    public class LinkTestImportVM : BaseImportVM<LinkTestTemplateVM, LinkTest>
    {

    }

}

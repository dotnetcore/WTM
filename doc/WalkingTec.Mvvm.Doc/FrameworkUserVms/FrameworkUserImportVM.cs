using System.Collections.Generic;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Doc.FrameworkUserVms
{
    public class FrameworkUserTemplateVM : BaseTemplateVM
    {
        public ExcelPropety c1 = ExcelPropety.CreateProperty<FrameworkUser>(x => x.ITCode);
        public ExcelPropety c2 = ExcelPropety.CreateProperty<FrameworkUser>(x => x.Password);
        public ExcelPropety c3 = ExcelPropety.CreateProperty<FrameworkUser>(x => x.Name);
        public ExcelPropety c5 = ExcelPropety.CreateProperty<FrameworkUser>(x => x.Gender);
        public ExcelPropety c6 = ExcelPropety.CreateProperty<FrameworkUser>(x => x.CellPhone);
        public ExcelPropety c7 = ExcelPropety.CreateProperty<FrameworkUser>(x => x.HomePhone);
        public ExcelPropety c8 = ExcelPropety.CreateProperty<FrameworkUser>(x => x.Address);
        public ExcelPropety c9 = ExcelPropety.CreateProperty<FrameworkUser>(x => x.ZipCode);

        protected override void InitVM()
        {
            c9.DataType = ColumnDataType.ComboBox;
            c9.ListItems = new List<ComboSelectListItem>
            {
                new ComboSelectListItem{ Text="aaa", Value="aaa"},
                new ComboSelectListItem{ Text="bbb", Value="bbb"},
                new ComboSelectListItem{ Text="ccc", Value="ccc"},
                new ComboSelectListItem{ Text="ddd", Value="ddd"},
            };
        }

    }

    public class FrameworkUserImportVM : BaseImportVM<FrameworkUserTemplateVM, FrameworkUser>
    {
        public override bool BatchSaveData()
        {
            SetEntityList();
            foreach (var item in EntityList)
            {
                item.IsValid = true;
            }
            return base.BatchSaveData();
        }
    }

}

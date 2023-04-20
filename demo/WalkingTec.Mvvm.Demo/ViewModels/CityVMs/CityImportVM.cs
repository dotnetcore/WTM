﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.CityVMs
{
    public partial class CityTemplateVM : BaseTemplateVM
    {
        [Display(Name = "名称")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<City>(x => x.Name);
        [Display(Name = "test")]
        public ExcelPropety Test_Excel = ExcelPropety.CreateProperty<City>(x => x.Test);
        [Display(Name = "_Admin.Parent")]
        public ExcelPropety Parent_Excel = ExcelPropety.CreateProperty<City>(x => x.ParentId);

	    protected override async Task InitVM()
        {
            Parent_Excel.DataType = ColumnDataType.ComboBox;
            Parent_Excel.ListItems = DC.Set<City>().GetSelectListItems(Wtm, y => y.Name);
        }

    }

    public class CityImportVM : BaseImportVM<CityTemplateVM, City>
    {

    }

}

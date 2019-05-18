using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.ReactDemo.ViewModels.CityVMs
{
    public class CityTemplateVM : BaseTemplateVM
    {
        [Display(Name = "名称")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<City>(x => x.Name);
        public ExcelPropety Level_Excel = ExcelPropety.CreateProperty<City>(x => x.Level);
        public ExcelPropety Codel_Excel = ExcelPropety.CreateProperty<City>(x => x.Code);
        public ExcelPropety ParentCodel_Excel = ExcelPropety.CreateProperty<City>(x => x.ParentCode);

        protected override void InitVM()
        {
        }

    }

    public class CityImportVM : BaseImportVM<CityTemplateVM, City>
    {
        public override bool BatchSaveData()
        {
            SetEntityList();
            EntityList.Remove(EntityList.Where(x => x.Code == "100000").FirstOrDefault());
            foreach (var item in EntityList)
            {
                if (item.ParentCode != "100000")
                {
                    item.Parent = EntityList.Where(x => x.Code == item.ParentCode).FirstOrDefault();
                }
            }
            return base.BatchSaveData();
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ReactDemo.Models;


namespace WalkingTec.Mvvm.ReactDemo.ViewModels.SchoolVMs
{
    public partial class SchoolVM : BaseCRUDVM<School>
    {
        public Guid? Place2_Sheng
        {
            get
            {
                return Entity.Place2?.Parent?.Parent?.ID;
            }
        }
        public Guid? Place2_Shi
        {
            get
            {
                return Entity.Place2?.Parent?.ID;
            }
        }

        public SchoolVM()
        {
            SetInclude(x => x.Place);
            SetInclude(x => x.Place2);
            SetInclude(x => x.Place2.Parent.Parent);
            SetInclude(x => x.Majors);
        }

        protected override void InitVM()
        {
        }

        public override void DoAdd()
        {           
            base.DoAdd();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            base.DoEdit(updateAllFields);
        }

        public override void DoDelete()
        {
            base.DoDelete();
        }
    }
}

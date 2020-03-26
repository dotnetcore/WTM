using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.VueDemo.BasicData.ViewModels.SchoolVMs
{
    public partial class SchoolBatchVM : BaseBatchVM<School, School_BatchEdit>
    {
        public SchoolBatchVM()
        {
            ListVM = new SchoolListVM();
            LinkedVM = new School_BatchEdit();
        }

    }

	/// <summary>
    /// 批量编辑字段类
    /// </summary>
    public class School_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}

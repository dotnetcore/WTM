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
    public partial class SchoolApiBatchVM : BaseBatchVM<School, SchoolApi_BatchEdit>
    {
        public SchoolApiBatchVM()
        {
            ListVM = new SchoolApiListVM();
            LinkedVM = new SchoolApi_BatchEdit();
        }

    }

	/// <summary>
    /// 批量编辑字段类
    /// </summary>
    public class SchoolApi_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}

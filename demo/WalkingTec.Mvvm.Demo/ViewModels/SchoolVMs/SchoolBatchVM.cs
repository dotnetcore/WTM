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
    public class SchoolBatchVM : BaseBatchVM<School, School_BatchEdit>
    {
        public SchoolBatchVM()
        {
            ListVM = new SchoolListVM();
            LinkedVM = new School_BatchEdit();
        }

        protected override bool CheckIfCanDelete(Guid id, out string errorMessage)
        {
            errorMessage = null;
			return true;
        }
    }

	/// <summary>
    /// 批量编辑字段类
    /// </summary>
    public class School_BatchEdit : BaseVM
    {
        [Display(Name = "学校类型")]
        public SchoolTypeEnum? SchoolType { get; set; }

        protected override void InitVM()
        {
        }

    }

}

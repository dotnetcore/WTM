using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.CompanyVMs
{
    public class CompanyBatchVM : BaseBatchVM<Company, Company_BatchEdit>
    {
        public CompanyBatchVM()
        {
            ListVM = new CompanyListVM();
            LinkedVM = new Company_BatchEdit();
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
    public class Company_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.ApiDemo;


namespace WalkingTec.Mvvm.ApiDemo.ViewModels.TestModelVMs
{
    public class TestModelBatchVM : BaseBatchVM<TestModel, TestModel_BatchEdit>
    {
        public TestModelBatchVM()
        {
            ListVM = new TestModelListVM();
            LinkedVM = new TestModel_BatchEdit();
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
    public class TestModel_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.bbb;


namespace WalkingTec.Mvvm.Demo.ViewModels.TestRoleVms
{
    public class TestRoleBatchVM : BaseBatchVM<TestRole, TestRole_BatchEdit>
    {
        public TestRoleBatchVM()
        {
            ListVM = new TestRoleListVM();
            LinkedVM = new TestRole_BatchEdit();
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
    public class TestRole_BatchEdit : BaseVM
    {
        public int test { get; set; }
        public string abc { get; set; }
        protected override void InitVM()
        {
        }

    }

}

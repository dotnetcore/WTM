using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Demo.Models;


namespace WalkingTec.Mvvm.Demo.ViewModels.MyUserVMs
{
    public class MyUserBatchVM : BaseBatchVM<MyUser, MyUser_BatchEdit>
    {
        public MyUserBatchVM()
        {
            ListVM = new MyUserListVM();
            LinkedVM = new MyUser_BatchEdit();
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
    public class MyUser_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}

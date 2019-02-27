using System;
using WalkingTec.Mvvm.Core;


namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.ActionLogVMs
{
    public class ActionLogBatchVM : BaseBatchVM<ActionLog, ActionLog_BatchEdit>
    {
        public ActionLogBatchVM()
        {
            ListVM = new ActionLogListVM();
            LinkedVM = new ActionLog_BatchEdit();
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
    public class ActionLog_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}

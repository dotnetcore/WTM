// WTM默认页面 Wtm buidin page
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

    }

    public class ActionLog_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}

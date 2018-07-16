using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.ActionLogVMs
{
    public class ActionLogVM : BaseCRUDVM<ActionLog>
    {
        public ActionLogListVM ListVm { get; set; }
    }
}

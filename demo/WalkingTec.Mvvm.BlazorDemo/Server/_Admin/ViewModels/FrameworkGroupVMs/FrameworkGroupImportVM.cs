// WTM默认页面 Wtm buidin page
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc.Admin.ViewModels.FrameworkGroupVMs
{
    public class FrameworkGroupTemplateVM : BaseTemplateVM
    {
        public ExcelPropety r1 = ExcelPropety.CreateProperty<FrameworkGroup>(x => x.GroupCode);
        public ExcelPropety r2 = ExcelPropety.CreateProperty<FrameworkGroup>(x => x.GroupName); 
        public ExcelPropety r3 = ExcelPropety.CreateProperty<FrameworkGroup>(x => x.GroupRemark);
    }

    public class FrameworkGroupImportVM : BaseImportVM<FrameworkGroupTemplateVM, FrameworkGroup>
    {

    }
}

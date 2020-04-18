using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;

using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Support.Json;

namespace WalkingTec.Mvvm.Mvc
{
    public interface IBaseController
    {
        WTMContext WtmContext { get; set; }

        Configs ConfigInfo { get; }
        GlobalData GlobaInfo { get; }
        string CurrentCS { get; }

        DBTypeEnum? CurrentDbType { get;  }

        IDataContext DC { get; }

        IDistributedCache Cache { get; }

        string BaseUrl { get;  }

        IDataContext CreateDC(bool isLog = false);

        ModelStateDictionary ModelState { get; }

        IStringLocalizer Localizer { get; }
    }
}

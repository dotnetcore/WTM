using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    public interface IBaseController
    {
        Configs ConfigInfo { get; }
        GlobalData GlobaInfo { get; }
        string CurrentCS { get; set; }

        DBTypeEnum? CurrentDbType { get; set; }

        IDataContext DC { get; set; }
        LoginUserInfo LoginUserInfo { get; set; }

        IDistributedCache Cache { get; }

        string BaseUrl { get; set; }

        ActionLog Log { get; set; }

        IDataContext CreateDC(bool isLog = false);

        ModelStateDictionary ModelState { get; }
    }
}

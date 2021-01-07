using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace WalkingTec.Mvvm.Mvc
{
    public class MvcProgram
    {
        public static IStringLocalizer _localizer = Core.CoreProgram._localizer;

    }
}

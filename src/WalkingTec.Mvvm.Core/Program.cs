using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;

namespace WalkingTec.Mvvm.Core
{
    public class Program
    {
        public static IStringLocalizer _localizer { get; set; }

        public static IStringLocalizer _Callerlocalizer { get; set; }

        public static string[] Buildindll = new string[]
            {
                    "WalkingTec.Mvvm.Core",
                    "WalkingTec.Mvvm.Mvc",
                    "WalkingTec.Mvvm.Admin",
                    "WalkingTec.Mvvm.Taghelpers"
            };

    }
}

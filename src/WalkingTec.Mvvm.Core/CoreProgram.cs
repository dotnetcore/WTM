using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace WalkingTec.Mvvm.Core
{
    public class CoreProgram
    {
        public static IStringLocalizer _localizer { get; set; }


        public static string[] Buildindll = new string[]
            {
                    "WalkingTec.Mvvm.Core",
                    "WalkingTec.Mvvm.Mvc",
                    "WalkingTec.Mvvm.Admin",
                    "WalkingTec.Mvvm.Taghelpers"
            };

    }
}

using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace WalkingTec.Mvvm.Mvc.Admin
{
    public class Program
    {
        public static IStringLocalizer _localizer =
            new ResourceManagerStringLocalizerFactory(Options.Create<LocalizationOptions>(new LocalizationOptions { ResourcesPath = "Resources" }), new Microsoft.Extensions.Logging.LoggerFactory()).Create(typeof(Core.Program));

    }
}

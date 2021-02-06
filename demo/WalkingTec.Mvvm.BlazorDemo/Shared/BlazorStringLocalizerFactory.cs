using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace WalkingTec.Mvvm.BlazorDemo.Shared
{
    public class BlazorStringLocalizerFactory : IStringLocalizerFactory
    {
        public IStringLocalizer Create(Type resourceSource)
        {
            return CreateStringLocalizer();
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return CreateStringLocalizer();
        }

        public IStringLocalizer Create()
        {
            return CreateStringLocalizer();
        }


        protected virtual IStringLocalizer CreateStringLocalizer()
        {
            return new ResourceManagerStringLocalizerFactory(
                                   Options.Create(
                                       new LocalizationOptions
                                       {
                                           ResourcesPath = "Resources"
                                       })
                                       , new Microsoft.Extensions.Logging.LoggerFactory()
                                   )
                                   .Create(typeof(Program));
        }

    }
}

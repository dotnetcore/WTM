using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    public static class ActionDescriptionExtension
    {
        public static string GetDescription(this ActionDescriptionAttribute self)
        {
            var localizer = GlobalServices.GetService<IStringLocalizer<WalkingTec.Mvvm.Core.Program>>();
            if(localizer != null && string.IsNullOrEmpty(self.Description) == false)
            {
                return localizer[self.Description];
            }
            else
            {
                return self.Description;
            }
        }
    }
}

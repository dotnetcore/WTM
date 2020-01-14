using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Localization;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.Mvc
{
    public static class ActionDescriptionExtension
    {
        public static string GetDescription(this ActionDescriptionAttribute self, IBaseController controller)
        {
            return self.GetDescription(controller.GetType());
        }

        public static string GetDescription(this ActionDescriptionAttribute self, Type controllertype)
        {
            string rv = "";
            if (string.IsNullOrEmpty(self.Description) == false)
            {
                if (Core.Program.Buildindll.Any(x => controllertype.FullName.StartsWith(x)))
                {
                    if (Core.Program._localizer != null)
                    {
                        rv = Core.Program._localizer[self.Description];
                    }
                }
                else
                {
                    if (Core.Program._Callerlocalizer != null)
                    {
                        rv = Core.Program._Callerlocalizer[self.Description];
                    }
                }
            }
            return rv;

        }

    }
}

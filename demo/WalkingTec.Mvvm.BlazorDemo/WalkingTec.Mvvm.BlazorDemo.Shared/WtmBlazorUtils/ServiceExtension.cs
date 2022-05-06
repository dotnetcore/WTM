using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.ConfigOptions;
using WalkingTec.Mvvm.Core.Json;

namespace WtmBlazorUtils
{
    public static class NavigationExtension
    {
        public static string QueryString(this NavigationManager nav, string paramName)
        {
            var uri = nav.ToAbsoluteUri(nav.Uri);
            string paramValue = HttpUtility.ParseQueryString(uri.Query).Get(paramName);
            return paramValue ?? "";
        }
    }
}

using System.Web;
using Microsoft.AspNetCore.Components;

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

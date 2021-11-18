using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public abstract class BaseTag : TagHelper
    {
        public string FormatFuncName(string funcname, bool appendparameter = true)
        {
            if (funcname == null)
            {
                return null;
            }
            var rv = funcname;
            var ind = rv.IndexOf("(");
            if (ind > 0)
            {
                rv = rv.Substring(0, ind);
            }
            if (appendparameter == true)
            {
                rv += "(data)";
            }
            return rv;
        }
    }
}

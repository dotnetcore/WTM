using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:submitbutton", TagStructure = TagStructure.WithoutEndTag)]
    public class SubmitButtonTagHelper : BaseButtonTag
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("type", "submit");
            output.Attributes.SetAttribute(new TagHelperAttribute("lay-submit"));
            if (context.Items.ContainsKey("formid"))
            {
                this.Id = context.Items["formid"] + "submit";
                output.Attributes.SetAttribute("lay-filter", context.Items["formid"] + "filter");
            }
            if (string.IsNullOrEmpty(Text))
            {
                Text = "提交";
            }
            base.Process(context, output);
        }
    }
}

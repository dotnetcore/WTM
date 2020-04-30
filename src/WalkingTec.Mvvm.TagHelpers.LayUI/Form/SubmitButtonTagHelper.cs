using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:submitbutton", TagStructure = TagStructure.WithoutEndTag)]
    public class SubmitButtonTagHelper : BaseButtonTag
    {
        public string SubmitUrl { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("type", "submit");
            output.Attributes.SetAttribute(new TagHelperAttribute("lay-submit"));
            if (context.Items.ContainsKey("formid"))
            {
                if(string.IsNullOrEmpty(this.Id) == true){
                    this.Id = Guid.NewGuid().ToString().Replace("-","");
                }
                output.Attributes.SetAttribute("lay-filter", context.Items["formid"] + "filter");
            }
            if (string.IsNullOrEmpty(Text))
            {
                Text = Program._localizer["Submit"];
            }
            base.Process(context, output);
        }
    }
}

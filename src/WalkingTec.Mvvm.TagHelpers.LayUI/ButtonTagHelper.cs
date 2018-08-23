using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:button", TagStructure = TagStructure.WithoutEndTag)]
    public class ButtonTagHelper : BaseButtonTag
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Content.Clear();
            output.Attributes.SetAttribute("type", "button");
            base.Process(context, output);
        }

    }
}

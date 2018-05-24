using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:container", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ContainerTagHelper : BaseElementTag
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            //output.Attributes.SetAttribute("style", "margin:10px;");
            base.Process(context, output);
        }
    }
}

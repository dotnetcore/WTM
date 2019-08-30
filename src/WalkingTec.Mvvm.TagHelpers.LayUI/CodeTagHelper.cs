using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{

    [HtmlTargetElement("wt:code", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class CodeTagHelper : BaseElementTag
    {
        public new int? Height { get; set; }

        public string Title { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "pre";
            output.Attributes.SetAttribute("class", "layui-code");
            output.Attributes.SetAttribute("encode", true);
            if (Height.HasValue)
            {
                output.Attributes.SetAttribute("lay-height", $"{Height}px");
            }
            if (string.IsNullOrEmpty(Title) == false)
            {
                output.Attributes.SetAttribute("lay-title", Title);
            }
            base.Process(context, output);
        }
    }
}

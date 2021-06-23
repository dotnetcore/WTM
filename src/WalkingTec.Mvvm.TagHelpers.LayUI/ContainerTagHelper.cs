using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:container", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ContainerTagHelper : BaseElementTag
    {
        public AlignEnum? Align { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            switch (Align)
            {
                case AlignEnum.Left:
                    output.Attributes.SetAttribute("style", "width:100%;text-align:left");
                    break;
                case AlignEnum.Right:
                    output.Attributes.SetAttribute("style", "width:100%;text-align:right");
                    break;
                case AlignEnum.Center:
                    output.Attributes.SetAttribute("style", "width:100%;text-align:center");
                    break;
                default:
                    break;
            }

            //output.Attributes.SetAttribute("style", "margin:10px;");
            base.Process(context, output);
        }
    }
}

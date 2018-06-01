using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public enum QuoteStyleEnum { Default, Simple}
    [HtmlTargetElement("wt:quote", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class QuoteTagHelper : BaseElementTag
    {
        public QuoteStyleEnum QuoteStyle { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "blockquote";
            if (QuoteStyle == QuoteStyleEnum.Default)
            {
                output.Attributes.SetAttribute("class", "layui-elem-quote");
            }
            else
            {
                output.Attributes.SetAttribute("class", "layui-elem-quote layui-quote-nm");
            }
            base.Process(context, output);
        }

    }
}

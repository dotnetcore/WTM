using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{

    public enum CardThemeEnum {  Dark, White}

    [HtmlTargetElement("wt:card", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class CardTagHelper : BaseElementTag
    {
        public AlignEnum? Align { get; set; }
        public string Title { get; set; }
        public CardThemeEnum CardTheme { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            string style = "";
            switch (Align)
            {
                case AlignEnum.Left:
                    style = "width:100%;text-align:left;";
                    break;
                case AlignEnum.Right:
                    style = "width:100%;text-align:right;";
                    break;
                case AlignEnum.Center:
                    style = "width:100%;text-align:center;";
                    break;
                default:
                    break;
            }
            if (CardTheme == CardThemeEnum.White)
            {
                output.Attributes.SetAttribute("class", "layui-card");
                output.Attributes.SetAttribute("style", style);
                string pre = $@"
    <div class=""layui-card-header"">{Title}</div>
        <div class=""layui-card-body"">
";
                string post = $@"
</div>";
                output.PreElement.SetHtmlContent(pre);
                output.PostElement.SetHtmlContent(post);
            }
            if(CardTheme == CardThemeEnum.Dark)
            {
                style += "border: solid 1px #eee;";
                output.Attributes.SetAttribute("class", "layui-card");
                output.Attributes.SetAttribute("style", style);
                string pre = $@"
    <div class=""layui-card-header"" style=""background-color:#fafafa"">{Title}</div>
        <div class=""layui-card-body"">
";
                string post = $@"
</div>";
                output.PreContent.SetHtmlContent(pre);
                output.PostContent.SetHtmlContent(post);
                //output.Content.SetHtmlContent(pre + output.GetChildContentAsync().Result.GetContent() + post);
            }
            base.Process(context, output);
        }
    }
}

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:closebutton", TagStructure = TagStructure.WithoutEndTag)]
    public class CloseButtonTagHelper : BaseButtonTag
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(Text))
            {
                Text = Program._localizer["Close"];
            }
            Click = "ff.CloseDialog()";



            base.Process(context, output);
        }
    }
}

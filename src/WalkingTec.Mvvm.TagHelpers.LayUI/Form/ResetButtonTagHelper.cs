using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:resetbutton", TagStructure = TagStructure.WithoutEndTag)]
    public class ResetButtonTagHelper : BaseButtonTag
    {
        public string ButtonText { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Attributes.SetAttribute("type", "reset");
            if (string.IsNullOrEmpty(Text))
            {
                Text = Program._localizer["Reset"];
            }
            base.Process(context, output);
        }
    }
}

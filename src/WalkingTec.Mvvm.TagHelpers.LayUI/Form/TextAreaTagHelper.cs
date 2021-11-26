using System.Net;
using System.Web;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:textarea", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.NormalOrSelfClosing)]
    public class TextAreaTagHelper : BaseFieldTag
    {
        public string EmptyText { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string placeHolder = EmptyText ?? "";
            output.TagName = "textarea";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("placeholder", placeHolder);
            output.Attributes.Add("class", "layui-textarea");
            if (string.IsNullOrEmpty(Field?.Model?.ToString()) == false)
            {
                DefaultValue = null;
            }
            if (DefaultValue != null)
            {
                output.Content.SetContent(WebUtility.HtmlDecode(DefaultValue.ToString()));
            }
            else
            {
                output.Content.SetContent(WebUtility.HtmlDecode(Field?.Model?.ToString()));
            }

            base.Process(context, output);
        }
    }
}

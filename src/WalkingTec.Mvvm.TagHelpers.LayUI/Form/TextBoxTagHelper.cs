using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:textbox", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class TextBoxTagHelper : BaseFieldTag
    {
        public string EmptyText { get; set; }

        public string SearchUrl { get; set; }


        public bool IsPassword { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string placeHolder = EmptyText ?? "";
            string type = IsPassword ? "password":"text";
            output.TagName = "input";
            output.TagMode = TagMode.StartTagOnly;
            output.Attributes.Add("type", type);
            output.Attributes.Add("name", Field.Name);
            if (DefaultValue != null)
            {
                output.Attributes.Add("value", DefaultValue);
            }
            else
            {
                output.Attributes.Add("value", Field?.Model);
            }
            output.Attributes.Add("placeholder", placeHolder);
            output.Attributes.Add("class", "layui-input");
            base.Process(context, output);
        }
    }

}

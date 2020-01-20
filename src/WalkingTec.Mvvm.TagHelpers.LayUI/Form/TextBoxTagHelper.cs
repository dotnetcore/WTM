using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:textbox", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class TextBoxTagHelper : BaseFieldTag
    {
        public string EmptyText { get; set; }

        public string SearchUrl { get; set; }

        public ModelExpression LinkField { get; set; }

        public string TriggerUrl { get; set; }


        /// <summary>
        /// 改变选择时触发的js函数，func(data)格式;
        /// <para>
        /// data.Text得到选中文本;
        /// </para>
        /// <para>
        /// data.Value得到被选中的值;
        /// </para>
        /// </summary>
        public string ChangeFunc { get; set; }


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
            if (string.IsNullOrEmpty(SearchUrl) == false)
            {
                output.Attributes.Add("autocomplete", "off");
            }
            base.Process(context, output);
        }
    }

}

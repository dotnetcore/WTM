using System.Net;
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
        public string LinkId { get; set; }
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
            output.Attributes.Add("wtm-name", Field.Name);
            if (string.IsNullOrEmpty(Field?.Model?.ToString()) == false)
            {
                DefaultValue = null;
            }
            if (DefaultValue != null)
            {
                output.Attributes.Add("value", WebUtility.HtmlDecode(DefaultValue));
            }
            else
            {
                output.Attributes.Add("value", WebUtility.HtmlDecode(Field?.Model?.ToString()));
            }
            output.Attributes.Add("placeholder", placeHolder);
            output.Attributes.Add("class", "layui-input");
            if (string.IsNullOrEmpty(SearchUrl) == false)
            {
                output.Attributes.Add("autocomplete", "off");
            }
            if (LinkField != null || string.IsNullOrEmpty(LinkId) == false)
            {
                var linkto = "";
                if (string.IsNullOrEmpty(LinkId))
                {
                    linkto = Core.Utils.GetIdByName(LinkField.ModelExplorer.Container.ModelType.Name + "." + LinkField.Name);
                }
                else
                {
                    linkto = LinkId;
                }
                output.Attributes.Add("wtm-linkto", $"{linkto}");
            }

            base.Process(context, output);
        }
    }

}

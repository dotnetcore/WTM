using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:switch", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class SwitchTagHelper : BaseFieldTag
    {
        /// <summary>
        /// 可自定义值
        /// 选中时返回的就是默认的on
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 可设定默认开
        /// </summary>
        public bool? Checked { get; set; }
        /// 可自定义开关两种状态的文本
        /// 例：ON|OFF 开启|关闭等
        /// </summary>
        public string LayText { get; set; }

        /// <summary>
        /// 改变选择时触发的js函数，func(data)格式;
        /// <para>
        /// data.elem得到checkbox原始DOM对象
        /// </para>
        /// <para>
        /// data.elem.checked是否被选中，true或者false
        /// </para>
        /// <para>
        /// value开关value值，也可以通过data.elem.value得到
        /// </para>
        /// <para>
        /// othis得到美化后的DOM对象
        /// </para>
        /// </summary>
        public string ChangeFunc { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "input";
            output.TagMode = TagMode.StartTagOnly;
            Value = string.IsNullOrEmpty(Value) ? "true" : Value;
            output.Attributes.Add("type", "checkbox");
            output.Attributes.Add("name", Field.Name);
            output.Attributes.Add("lay-skin", "switch");
            output.Attributes.Add("value", Value);
            output.Attributes.Add("lay-text", string.IsNullOrEmpty(LayText) ? "ON|OFF" : LayText);

            Checked = Field.Model == null ? Checked : (Field.Model.ToString().ToLower() == Value.ToLower());
            if (Checked.HasValue && Checked.Value)
            {
                output.Attributes.Add("checked", string.Empty);
            }

            base.Process(context, output);

        }
    }
}

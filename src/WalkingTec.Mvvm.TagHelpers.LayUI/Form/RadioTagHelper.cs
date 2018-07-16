using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections;
using System.Collections.Generic;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:radio", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class RadioTagHelper : BaseFieldTag
    {
        public string YesText { get; set; }

        public string NoText { get; set; }

        public ModelExpression Items { get; set; }

        /// <summary>
        /// 改变选择时触发的js函数，func(data)格式;
        /// <para>
        /// data.elem得到radio原始DOM对象
        /// </para>
        /// <para>
        /// data.value被点击的radio的value值
        /// </para>
        /// </summary>
        public string ChangeFunc { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "input";
            output.TagMode = TagMode.StartTagOnly;
            output.Attributes.Add("type", "radio");
            output.Attributes.Add("name", Field.Name);

            var modeltype = Field.Metadata.ModelType;
            var listitems = new List<ComboSelectListItem>();
            if (Items?.Model == null)
            {
                if (modeltype.IsEnumOrNullableEnum())
                {
                    listitems = modeltype.ToListItems(Field.Model);
                }
                else if (modeltype == typeof(bool) || modeltype == typeof(bool?))
                {
                    listitems = Utils.GetBoolCombo(BoolComboTypes.Custom, (bool?)Field.Model, YesText, NoText);
                }

            }
            else
            {
                if (Items.Metadata.ModelType == typeof(List<ComboSelectListItem>))
                {
                    listitems = Items.Model as List<ComboSelectListItem>;
                    foreach (var item in listitems)
                    {
                        if (item.Value.ToLower() == Field.Model?.ToString().ToLower())
                        {
                            item.Selected = true;
                        }
                    }
                }
                else if (Items.Metadata.ModelType.IsList())
                {
                    var exports = (Items.Model as IList);
                    foreach (var item in exports)
                    {
                        ComboSelectListItem newitem = new ComboSelectListItem();
                        newitem.Text = item?.ToString();
                        newitem.Value = item?.ToString();
                        if (item == Field.Model)
                        {
                            newitem.Selected = true;
                        }
                        listitems.Add(newitem);
                    }
                }
            }
            if (listitems.Count > 0)
            {
                output.Attributes.Add("value", listitems[0].Value);
                output.Attributes.Add(new TagHelperAttribute("title", listitems[0].Text));
                if (listitems[0].Selected)
                {
                    output.Attributes.Add("checked", null);
                }
            }
            for (int i = 1; i < listitems.Count; i++)
            {
                var item = listitems[i];
                var selected = item.Selected ? " checked" : " ";
                output.PostElement.AppendHtml($@"
        <input type=""radio"" name=""{Field.Name}"" value=""{item.Value}"" title=""{item.Text}"" {selected} />");
            }

            base.Process(context, output);

        }
    }
}

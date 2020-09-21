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
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Clear();
            output.Attributes.Add("div-for", "radio");
            output.Attributes.Add("wtm-ctype", "radio");

            var modeltype = Field.Metadata.ModelType;
            var listItems = new List<ComboSelectListItem>();
            if (Items?.Model == null)
            {
                var checktype = modeltype;
                if ((modeltype.IsGenericType && typeof(List<>).IsAssignableFrom(modeltype.GetGenericTypeDefinition())))
                {
                    checktype = modeltype.GetGenericArguments()[0];
                }

                if (checktype.IsEnumOrNullableEnum())
                {
                    listItems = checktype.ToListItems(DefaultValue ?? Field.Model);
                }
                else if (checktype == typeof(bool) || checktype == typeof(bool?))
                {
                    bool? df = null;
                    if (bool.TryParse(DefaultValue ?? "", out bool test) == true)
                    {
                        df = test;
                    }
                    listItems = Utils.GetBoolCombo(BoolComboTypes.Custom, df ?? (bool?)Field.Model, YesText, NoText);
                }

            }
            else
            {
                string sv = "";
                if(DefaultValue == null)
                {
                    sv = Field.Model?.ToString();
                }
                else
                {
                    sv = DefaultValue;
                }

                if (Items.Metadata.ModelType == typeof(List<ComboSelectListItem>))
                {
                    listItems = Items.Model as List<ComboSelectListItem>;
                    foreach (var item in listItems)
                    {
                        if (item.Value.ToString().ToLower() == sv?.ToLower())
                        {
                            item.Selected = true;
                        }
                        else
                        {
                            item.Selected = false;
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
                        if (item?.ToString() == sv)
                        {
                            newitem.Selected = true;
                        }
                        listItems.Add(newitem);
                    }
                }
            }

            for (int i = 0; i < listItems.Count; i++)
            {
                var item = listItems[i];
                var selected = item.Selected ? " checked" : " ";
                output.PostContent.AppendHtml($@"
        <input type=""radio"" name=""{Field.Name}"" value=""{item.Value}"" title=""{item.Text}"" {selected} />");
            }

            base.Process(context, output);

        }
    }
}

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
            output.Attributes.Add("wtm-name", Field.Name);

            var modeltype = Field.Metadata.ModelType;
            var listItems = new List<ComboSelectListItem>();
            List<string> values = new List<string>();
            if (modeltype.IsBoolOrNullableBool())
            {
                if (Field.Model == null)
                {
                    values.Add("False");
                }
                else
                {
                    values.Add(Field.Model.ToString());
                }
            }
            else
            {
                if (Field.Model != null)
                {
                    values.Add(Field.Model.ToString());
                }
            }

            if (values.Count == 0)
            {
                if (DefaultValue != null)
                {
                    values = DefaultValue.Split(',').ToList();
                }

            }

            if (string.IsNullOrEmpty(ItemUrl) == false)
            {
                output.PostElement.AppendHtml($"<script>ff.LoadComboItems('radio','{ItemUrl}','{Id}','{Field.Name}',{JsonSerializer.Serialize(values)})</script>");
            }
            else
            {
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
                    if (typeof(IEnumerable<ComboSelectListItem>).IsAssignableFrom(Items.Metadata.ModelType))
                    {
                        if (typeof(IEnumerable<TreeSelectListItem>).IsAssignableFrom(Items.Metadata.ModelType))
                        {
                            listItems = (Items.Model as IEnumerable<TreeSelectListItem>).FlatTreeSelectList().Cast<ComboSelectListItem>().ToList();
                        }
                        else
                        {
                            listItems = (Items.Model as IEnumerable<ComboSelectListItem>).ToList();
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
                            listItems.Add(newitem);
                        }
                    }
                }
                SetSelected(listItems, values);
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

        private void SetSelected(List<ComboSelectListItem> source, IList data)
        {
            if (data == null)
            {
                return;
            }
            var textAndValue = false;
            if (data.GetType().GetGenericArguments()[0] == typeof(ComboSelectListItem))
            {
                textAndValue = true;
            }
            foreach (var item in source)
            {
                foreach (var item2 in data)
                {
                    if (textAndValue == true)
                    {
                        if (item.Value.ToString().ToLower() == (item2 as ComboSelectListItem).Value.ToString().ToLower())
                        {
                            item.Selected = true;
                            break;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }
                    else
                    {
                        if (item.Value.ToString().ToLower() == item2?.ToString().ToLower())
                        {
                            item.Selected = true;
                            break;
                        }
                        else
                        {
                            item.Selected = false;
                        }
                    }
                }
            }

        }

    }
}

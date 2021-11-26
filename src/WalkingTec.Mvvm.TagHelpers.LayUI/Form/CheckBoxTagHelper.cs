using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Attributes;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:checkbox", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class CheckBoxTagHelper : BaseFieldTag
    {

        /// <summary>
        /// 选项
        /// </summary>
        public ModelExpression Items { get; set; }

        /// <summary>
        /// 改变选择时触发的js函数，func(data)格式;
        /// <para>
        /// data.elem得到checkbox原始DOM对象
        /// </para>
        /// <para>
        /// data.elem.checked是否被选中，true或者false
        /// </para>
        /// <para>
        /// data.value复选框value值，也可以通过data.elem.value得到
        /// </para>
        /// <para>
        /// othis得到美化后的DOM对象
        /// </para>
        /// </summary>
        public string ChangeFunc { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var modelType = Field.Metadata.ModelType;
            var listItems = new List<ComboSelectListItem>();
            List<string> values = new List<string>();
            if (Field?.Name?.Contains("[") == true)
            {
                values.AddRange(Field.ModelExplorer.Container.Model.GetPropertySiblingValues(Field.Name));
            }
            else
            {
                if (modelType.IsList())
                {
                    var ilist = Field.Model as IList;
                    if (ilist != null)
                    {
                        foreach (var item in ilist)
                        {
                            values.Add(item.ToString());
                        }
                    }
                }
                else if (modelType.IsBoolOrNullableBool())
                {
                    values.Add(Field.Model.ToString());
                }
                else
                {
                    if (Field.Model != null)
                    {
                        values.Add(Field.Model.ToString());
                    }
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
                output.PostElement.AppendHtml($"<script>ff.LoadComboItems('checkbox','{ItemUrl}','{Id}','{Field.Name}',{JsonSerializer.Serialize(values)})</script>");
            }
            else
            {

                if (Items?.Model == null)
                {
                    if (modelType.IsList())
                    {
                        var innerType = modelType.GetGenericArguments()[0];
                        if (innerType.IsEnumOrNullableEnum())
                        {
                            listItems = innerType.ToListItems();
                        }
                    }
                    else if (modelType.IsBoolOrNullableBool())
                    {
                        listItems = new List<ComboSelectListItem>() { new ComboSelectListItem { Value = "true", Text = "|" } };
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
                            listItems.Add(new ComboSelectListItem
                            {
                                Text = item?.ToString(),
                                Value = item?.ToString()
                            });
                        }
                    }
                    try
                    {
                        List<string> checkvalue = null;
                        //如果是Entity.xxList[0].xxxid的格式，使用GetPropertySiblingValues方法获取Entity.xxxList.Select(x=>x.xxxid).ToList()的结果
                        if (Field.Name.Contains("["))
                        {
                            //默认多对多不必填
                            if (Required == null)
                            {
                                Required = false;
                            }
                        }
                        else if (Field.Model is IList == false && Field.Model != null)
                        {
                            checkvalue = new List<string> { Field.Model.ToString() };
                        }
                        else
                        {
                        }
                    }
                    catch
                    {
                    }
                }
                SetSelected(listItems, values);
            }
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Clear();
            output.Attributes.Add("div-for", "checkbox");
            output.Attributes.Add("wtm-ctype", "checkbox");
            output.Attributes.Add("wtm-name", Field.Name);

            if (string.IsNullOrEmpty(ChangeFunc) == false)
            {
                output.Attributes.Add("wtm-cf", FormatFuncName(ChangeFunc, false));
            }
            for (int i = 0; i < listItems.Count; i++)
            {
                var item = listItems[i];
                var selected = item.Selected ? " checked" : " ";
                output.PostContent.AppendHtml($@"
<input type=""checkbox"" name=""{Field.Name}"" value=""{item.Value}"" title=""{item.Text}"" {selected} {(Disabled ? "disabled=\"\"" : string.Empty)}/>");
            }
            output.PostElement.AppendHtml($@"<input type=""hidden"" name=""_DONOTUSE_{Field.Name}"" value=""1"" />");
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

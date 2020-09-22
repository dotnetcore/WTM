using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            var listitems = new List<ComboSelectListItem>();
            List<string> values = null;
            if(DefaultValue != null)
            {
                values = DefaultValue.Split(',').ToList();
            }
            var middleTable = modelType.GetCustomAttributes(typeof(MiddleTableAttribute), false).FirstOrDefault();
            //如果指向多对多中间表
            if(middleTable != null)
            {

            }
            if (Items?.Model == null)
            {
                if (modelType.IsList())
                {
                    var innerType = modelType.GetGenericArguments()[0];
                    if (innerType.IsEnumOrNullableEnum())
                    {
                        listitems = innerType.ToListItems();
                        SetSelected(listitems, values ?? Field.Model as IList);
                    }
                }
                else if (modelType.IsBoolOrNullableBool())
                {
                    listitems = new List<ComboSelectListItem>() { new ComboSelectListItem { Value = "true", Text = "|", Selected = Field.Model?.ToString().ToLower() == "true" } };
                }
            }
            else
            {
                if (Items.Metadata.ModelType == typeof(List<ComboSelectListItem>))
                {
                    listitems = Items.Model as List<ComboSelectListItem>;
                }
                else if (Items.Metadata.ModelType.IsList())
                {
                    var exports = (Items.Model as IList);
                    foreach (var item in exports)
                    {
                        listitems.Add(new ComboSelectListItem
                        {
                            Text = item?.ToString(),
                            Value = item?.ToString()
                        });
                    }
                }
                List<string> checkvalue = null;
                if (Field.Model is IList == false && Field.Model != null)
                {
                    checkvalue = new List<string> { Field.Model.ToString() };
                    SetSelected(listitems, values ?? checkvalue);
                }
                else
                {
                    SetSelected(listitems, values ?? Field.Model as IList);
                }
            }

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Clear();
            output.Attributes.Add("div-for", "checkbox");
            output.Attributes.Add("wtm-ctype", "checkbox");
            if (string.IsNullOrEmpty(ChangeFunc) == false)
            {
                output.Attributes.Add("wtm-cf", FormatFuncName(ChangeFunc, false));
            }
            for (int i = 0; i < listitems.Count; i++)
            {
                var item = listitems[i];
                var selected = item.Selected ? " checked" : " ";
                output.PostContent.AppendHtml($@"
<input type=""checkbox"" name=""{Field.Name}"" value=""{item.Value}"" title=""{item.Text}"" {selected} {(Disabled ? "disabled=\"\"" : string.Empty)}/>");
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

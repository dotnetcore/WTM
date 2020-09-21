using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:combobox", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class ComboBoxTagHelper : BaseFieldTag
    {
        public string EmptyText { get; set; }

        public bool AutoComplete { get; set; }

        public string YesText { get; set; }

        public string NoText { get; set; }

        /// <summary>
        /// 启用搜索
        /// 注意：多选与搜索不能同时启用
        /// </summary>
        public bool EnableSearch { get; set; }

        public ModelExpression Items { get; set; }

        public ModelExpression LinkField { get; set; }

        public string TriggerUrl { get; set; }

        /// <summary>
        /// 是否多选
        /// 默认根据Field 绑定的值类型进行判断。Array or List 即多选，否则单选
        /// 注意：多选与搜索不能同时启用
        /// </summary>
        public bool? MultiSelect { get; set; }

        /// <summary>
        /// 改变选择时触发的js函数，func(data)格式;
        /// <para>
        /// data.elem得到select原始DOM对象;
        /// </para>
        /// <para>
        /// data.value得到被选中的值;
        /// </para>
        /// <para>
        /// data.othis得到美化后的DOM对象;
        /// </para>
        /// </summary>
        public string ChangeFunc { get; set; }

        public ComboBoxTagHelper()
        {
            if (EmptyText == null)
            {
                EmptyText = Program._localizer["PleaseSelect"];
            }
            EnableSearch = GlobalServices.GetRequiredService<Configs>().UiOptions.ComboBox.DefaultEnableSearch;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "select";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("name", Field.Name);
            output.Attributes.Add("lay-filter", Field.Name);
            output.Attributes.Add("wtm-name", Field.Name);
            output.Attributes.Add("wtm-ctype", "combo");
            if (Disabled == true)
            {
                output.Attributes.Add("disabled", "disabled");

            }

            if (MultiSelect == null)
            {
                MultiSelect = false;
                var type = Field.Metadata.ModelType;
                if (type.IsArray || (type.IsGenericType && typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition())))// Array or List
                {
                    MultiSelect = true;
                }
            }
            if (MultiSelect.Value)
            {
                output.Attributes.Add("wtm-combo", "MULTI_COMBO");
            }
            if (!MultiSelect.Value && EnableSearch)
            {
                output.Attributes.Add("lay-search", string.Empty);
            }
            if (Field.Metadata.IsRequired)
            {
                output.Attributes.Add("lay-verify", MultiSelect.Value ? "selectRequired" : "required");
            }
            if (string.IsNullOrEmpty(ChangeFunc) == false)
            {
                output.Attributes.Add("wtm-cf", FormatFuncName(ChangeFunc, false));
            }
            if (LinkField != null)
            {
                output.Attributes.Add("wtm-linkto", $"{Core.Utils.GetIdByName(LinkField.ModelExplorer.Container.ModelType.Name + "." + LinkField.Name)}");
                output.Attributes.Add("wtm-tname", $"{LinkField.Name}");
            }
            if (TriggerUrl != null)
            {
                output.Attributes.Add("wtm-turl", TriggerUrl);
            }
            var contentBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(EmptyText) == false)
            {
                contentBuilder.Append($"<option value=''>{(Disabled == true ? "" : EmptyText)}</option>");
            }

            #region 添加下拉数据 并 设置默认选中

            var modeltype = Field.Metadata.ModelType;
            var listItems = new List<ComboSelectListItem>();

            if (Items?.Model == null) // 添加默认下拉数据源
            {
                var checktype = modeltype;
                if ((modeltype.IsGenericType && typeof(List<>).IsAssignableFrom(modeltype.GetGenericTypeDefinition())))
                {
                    checktype = modeltype.GetGenericArguments()[0];
                }

                if (checktype.IsEnumOrNullableEnum())
                {
                    listItems = checktype.ToListItems(DefaultValue?? Field.Model);
                }
                else if (checktype == typeof(bool) || checktype == typeof(bool?))
                {
                    bool? df = null;
                    if(bool.TryParse(DefaultValue ?? "",out bool test) == true)
                    {
                        df = test;
                    }
                    listItems = Utils.GetBoolCombo(BoolComboTypes.Custom,df ?? (bool?)Field.Model, YesText, NoText);
                }
            }
            else // 添加用户设置的设置源
            {
                var selectVal = new List<string>();
                if (DefaultValue == null)
                {
                    if (Field.Model != null)
                    {
                        if (modeltype.IsArray || (modeltype.IsGenericType && typeof(List<>).IsAssignableFrom(modeltype.GetGenericTypeDefinition())))
                        {
                            foreach (var item in Field.Model as dynamic)
                            {
                                selectVal.Add(item.ToString().ToLower());
                            }
                        }
                        else
                        {
                            selectVal.Add(Field.Model.ToString().ToLower());
                        }
                    }
                }
                else
                {
                    selectVal.AddRange(DefaultValue.Split(',').Select(x=>x.ToLower()));
                }
                if (Items.Metadata.ModelType == typeof(List<ComboSelectListItem>))
                {
                    listItems = Items.Model as List<ComboSelectListItem>;
                    foreach (var item in listItems)
                    {
                        if (selectVal.Contains(item.Value?.ToString().ToLower()))
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
                        listItems.Add(new ComboSelectListItem
                        {
                            Text = item?.ToString(),
                            Value = item?.ToString(),
                            Selected = selectVal.Contains(item?.ToString())
                        });
                    }
                }
            }

            if (MultiSelect.Value)
            {
                foreach (var item in listItems)
                {
                    contentBuilder.Append($"<option value='{item.Value}'{(string.IsNullOrEmpty(item.ICon) ? string.Empty : $" icon='{item.ICon}'")}>{item.Text}</option>");
                }

                // 添加默认选中项
                var selected = listItems.Where(x => x.Selected).ToList();
                var mulvalues = selected.ToSpratedString(x => x.Value, seperator: "`");
                var mulnamess = selected.ToSpratedString(x => x.Text, seperator: "`");
                output.Attributes.Add("wtm-combovalue", $"{mulvalues}");
                output.Attributes.Add("wtm-comboname", $"{mulnamess}");
            }
            else // 添加用户设置的设置源
            {
                foreach (var item in listItems)
                {
                    if (item.Selected == true)
                    {
                        if (Disabled == true)
                        {
                            output.PostElement.AppendHtml($"<input name='{Field.Name}' value='{item.Value}' text='{item.Text}' type='hidden' />");
                        }
                        contentBuilder.Append($"<option value='{item.Value}'{(string.IsNullOrEmpty(item.ICon) ? string.Empty : $" icon='{item.ICon}'")} selected>{item.Text}</option>");
                    }
                    else
                    {
                        contentBuilder.Append($"<option value='{item.Value}'{(string.IsNullOrEmpty(item.ICon) ? string.Empty : $" icon='{item.ICon}'")} {(Disabled && listItems.Count > 1 && Field.Model != null ? "disabled=\"\"" : string.Empty)}>{item.Text}</option>");
                    }
                }
            }
            output.Content.SetHtmlContent(contentBuilder.ToString());

            #endregion

            base.Process(context, output);
        }
    }
}

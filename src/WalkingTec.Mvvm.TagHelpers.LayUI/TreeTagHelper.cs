using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:tree",  TagStructure = TagStructure.WithoutEndTag)]
    public class TreeTagHelper : BaseFieldTag
    {
        public string EmptyText { get; set; }
        public ModelExpression Items { get; set; }
        public bool ShowLine { get; set; } = true;
        /// <summary>
        /// 勾选事件
        /// </summary>
        /// <summary>
        /// 勾选时触发的js函数名，func(data)格式;
        /// <para>
        /// data.arr得到当前选中数据数组;
        /// </para>
        /// <para>
        /// data.change得到本次操作变化的数据数组
        /// </para>
        /// <para>
        /// data.isadd得到本次操作是增加还是删除
        /// </para>
        /// </summary>
        public string ChangeFunc { get; set; }
        public bool AutoRow { get; set; }
        public bool? EnableSearch { get; set; }
        public bool? ShowToolbar { get; set; } = true;
        public ModelExpression LinkField { get; set; }

        public string LinkId { get; set; }
        public string TriggerUrl { get; set; }

        public TreeTagHelper(IOptionsMonitor<Configs> configs)
        {
            if (EmptyText == null)
            {
                EmptyText = THProgram._localizer["Sys.PleaseSelect"];
            }
            if (EnableSearch == null)
            {
                EnableSearch = configs.CurrentValue.UIOptions.ComboBox.DefaultEnableSearch;
            }
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            bool MultiSelect = false;
            var type = Field.Metadata.ModelType;
            if (Field.Name.Contains("[") || type.IsArray || type.IsList())// Array or List
            {
                MultiSelect = true;
            }

            output.TagName = "div";
            output.Attributes.Add("id", Id);
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("wtm-ctype", "tree");
            output.Attributes.Add("wtm-name", Field.Name);
            output.Attributes.Add("wtm-multi", MultiSelect.ToString().ToLower());
            Id = string.IsNullOrEmpty(Id) ? Guid.NewGuid().ToNoSplitString() : Id;

                List<object> vals = new List<object>();
                if (Field?.Model != null)
                {
                    if (MultiSelect == true)
                    {
                        foreach (var item in Field.Model as dynamic)
                        {
                            vals.Add(item.ToString());
                        }
                    }
                    else
                    {
                        vals.Add(Field.Model.ToString());
                    }
                }
                List<LayuiTreeItem> treeitems = new List<LayuiTreeItem>();

                if (string.IsNullOrEmpty(ItemUrl) == true && Items?.Model is List<TreeSelectListItem> mm)
                {
                    treeitems = GetLayuiTree(mm, vals);
                }

            if (string.IsNullOrEmpty(ItemUrl) == false)
            {
                foreach (var item in vals)
                {
                    treeitems.Add(new  LayuiTreeItem
                    {
                        Title = "",
                        Id = item?.ToString(),
                        Checked = true
                    });

                }
                output.PostElement.AppendHtml($@"<script>
ff.LoadComboItems('tree','{ItemUrl}','{Id}','{Field.Name}',{JsonSerializer.Serialize(vals)},function(){{
}})

</script>");
            }

            var script = $@"
<script>
var {Id} = xmSelect.render({{
    el: '#{Id}',
    name:'{Field.Name}',
    tips:'{EmptyText}',
    disabled: {Disabled.ToString().ToLower()},
    {(THProgram._localizer["Sys.LayuiDateLan"] == "CN" ? "language:'zn'," : "language:'en',")}
	autoRow: {AutoRow.ToString().ToLower()},
	filterable: {EnableSearch.ToString().ToLower()},
    template({{ item, sels, name, value }}){{
        if(item.icon !== undefined && item.icon != """"&& item.icon != null){{
			return '<i class=""'+item.icon+'""></i>' + item.name;
        }}
        else{{
            return item.name;
        }}
	}},
    {(MultiSelect == false ? $@"
    radio: true,
    clickClose: true,
    model: {{
        label: {{
            type: 'abc' ,
            abc: {{
                template: function(item, sels){{
                    if(sels[0].icon !== undefined && sels[0].icon != """" && sels[0].icon != null){{
                        return '<i class=""'+sels[0].icon+'""></i>' + sels[0].name;
                    }}
                    else{{
                        return sels[0].name;
                    }}
                }}
            }}
        }}
    }},
    toolbar: {{
        show: {ShowToolbar.Value.ToString().ToLower()},
        list: ['CLEAR']}}," : $@"
        toolbar: {{show: true,list: ['ALL', 'REVERSE', 'CLEAR']}},
        model: {{
		label: {{
			block: {{
				template: function(item, sels){{
                    if(item.icon !== undefined && item.icon != """"&& item.icon != null){{
					    return '<i class=""'+item.icon+'""></i>' + item.name;
                    }}
                    else{{
                        return item.name;
                    }}
				}},
			}},
		}}
	}},
")}	tree: {{
        strict: true,
		show: true,
		showFolderIcon: true,
		showLine: true,
		indent: 20
	}},
	height: '400px',
    on:function(data){{
        {((LinkField != null || string.IsNullOrEmpty(LinkId) == false) ? @$"
            if (eval(""{(string.IsNullOrEmpty(ChangeFunc) ? "1==1" : FormatFuncName(ChangeFunc))}"") != false) {{
                var u = ""{(TriggerUrl ?? "")}"";
                if (u.indexOf(""?"") == -1) {{
                    u += ""?t="" + new Date().getTime();
                }}
                for (var i = 0; i < data.arr.length; i++) {{
                    u += ""&id="" + data.arr[i].value;
                }}
                ff.ChainChange(u, $('#{Id}')[0])
        }}" : FormatFuncName(ChangeFunc))}
   }},
	data:  {JsonSerializer.Serialize(treeitems)}
}})
</script>
";
                output.PostElement.AppendHtml(script);
                string hidden = $"<p id='tree{Id}hidden'>";
                if (Field?.Model != null)
                {
                    if (MultiSelect == true)
                    {
                        foreach (var item in Field.Model as dynamic)
                        {
                            hidden += $@"
<input type='hidden' name='{Field?.Name}' value='{item.ToString()}'/>";
                        }
                    }
                    else
                    {
                        hidden += $"<input type='hidden' name='{Field?.Name}' value='{Field.Model}'/>";
                    }
                    hidden += " </p>";
                }

                output.PostElement.AppendHtml(hidden);

            base.Process(context, output);
        }

        private List<LayuiTreeItem> GetLayuiTree(IEnumerable<TreeSelectListItem> tree, List<object> values)
        {
            List<LayuiTreeItem> rv = new List<LayuiTreeItem>();
            foreach (var s in tree)
            {
                var news = new LayuiTreeItem
                {
                    Id = s.Value.ToString(),
                    Title = s.Text,
                    Url = s.Url,
                    Expand = s.Expended,
                    Disabled = s.Disabled,
                    Icon = s.Icon
                    //Children = new List<LayuiTreeItem>()
                };
                if (values.Contains(s.Value.ToString()))
                {
                    news.Checked = true;
                }
                if (s.Children != null && s.Children.Count() > 0)
                {
                    news.Children = GetLayuiTree(s.Children, values);
                    if(news.Children.Where(x=>x.Checked == true || x.Expand == true).Count() > 0)
                    {
                        news.Expand = true;
                    }
                }
                rv.Add(news);
            }
            return rv;
        }


    }
}

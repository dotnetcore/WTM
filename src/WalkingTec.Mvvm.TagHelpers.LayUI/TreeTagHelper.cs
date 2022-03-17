using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using System.Linq;
using System.Text.Json;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:tree",  TagStructure = TagStructure.WithoutEndTag)]
    public class TreeTagHelper : BaseFieldTag
    {
        public string EmptyText { get; set; }
        public ModelExpression Items { get; set; }
        public bool ShowLine { get; set; } = true;
        /// <summary>
        /// 点击事件
        /// </summary>
        /// <summary>
        /// 点击时触发的js函数名，func(data)格式;
        /// <para>
        /// data.elem得到当前节点元素;
        /// </para>
        /// <para>
        /// data.data得到当前点击的节点数据
        /// </para>
        /// <para>
        /// data.state得到当前节点的展开状态：open、close、normal
        /// </para>
        /// </summary>
        public string ClickFunc { get; set; }
        /// <summary>
        /// 勾选事件
        /// </summary>
        /// <summary>
        /// 勾选时触发的js函数名，func(data)格式;
        /// <para>
        /// data.elem得到当前节点元素;
        /// </para>
        /// <para>
        /// data.data得到当前点击的节点数据
        /// </para>
        /// <para>
        /// data.checked是否被选中
        /// </para>
        /// </summary>
        public string CheckFunc { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            bool MultiSelect = false;
            var type = Field.Metadata.ModelType;
            if (Field.Name.Contains("[") || type.IsArray || type.IsList())// Array or List
            {
                MultiSelect = true;
            }
            if (EmptyText == null)
            {
                EmptyText = THProgram._localizer["Sys.PleaseSelect"];
            }

            output.TagName = "div";
            output.Attributes.Add("id", "div" + Id);
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("wtm-ctype", "tree");
            output.Attributes.Add("wtm-name", Field.Name);
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
                var script = $@"
<script>
var {Id} = xmSelect.render({{
    el: '#div{Id}',
    name:'{Field.Name}',
    tips:'{EmptyText}',
	autoRow: false,
	filterable: true,
    {(Required == true?$"layVerify: 'required',layReqText:'{THProgram._localizer["Validate.{0}required", Field?.Metadata?.DisplayName ?? Field?.Metadata?.Name]}'," :"")}    
    {(MultiSelect==false? "radio: true,":"")}
	tree: {{
        strict: false,
		show: true,
		showFolderIcon: true,
		showLine: true,
		indent: 20,
		expandedKeys: [ -3 ],
	}},
	toolbar: {{
        show: true,
		list: ['ALL', 'REVERSE', 'CLEAR']
	}},
	height: '400px',
	data:  {JsonSerializer.Serialize(treeitems)}
}})
</script>
";
                output.PostElement.AppendHtml(script);
                if (string.IsNullOrEmpty(ItemUrl) == false)
                {
                    output.PostElement.AppendHtml($@"<script>
ff.LoadComboItems('tree','{ItemUrl}','{Id}','{Field.Name}',{JsonSerializer.Serialize(vals)},function(){{
}})

</script>");
                }
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
                    Disabled = s.Disabled
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

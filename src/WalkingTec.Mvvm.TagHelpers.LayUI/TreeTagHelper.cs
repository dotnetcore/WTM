using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:tree", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class TreeTagHelper : BaseElementTag
    {
        protected const string REQUIRED_ATTR_NAME = "items";
        public ModelExpression Items { get; set; }
        /// <summary>
        /// 点击事件
        /// </summary>
        public string ClickFunc { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            Id = string.IsNullOrEmpty(Id) ? Guid.NewGuid().ToNoSplitString() : Id;
            if (Items.Model is List<TreeSelectListItem> mm)
            {
                var script = $@"
<script>
layui.tree({{
  elem: '#{Id}',nameField:'text',childrenField:'children',spreadField:'expended'
  {(string.IsNullOrEmpty(ClickFunc) ? string.Empty : $",click:function(a){{{ClickFunc}(a);}}")}
  ,nodes: {JsonConvert.SerializeObject(mm, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() })}
}});
</script>
";
                output.PostElement.AppendHtml(script);
            }
            else
            {
                output.Content.SetContent("无法绑定Tree，items参数必须设定为类型为List<ITreeData<>>的值");

            }
            base.Process(context, output);
        }

        private string GetTreeJson(List<TreeSelectListItem> tree)
        {
            var treeBuilder = new StringBuilder();
            for (int i = 0; i < tree.Count; i++)
            {
                var item = tree[i];
                treeBuilder.Append($"{{name:'{item.Text}'");
                if (item.Children?.Count > 0)
                {
                    treeBuilder.Append($",children:[{GetTreeJson(item.Children)}]");
                }
                treeBuilder.Append("}");
                if (i < tree.Count - 1)
                {
                    treeBuilder.Append(",");
                }
            }
            return treeBuilder.ToString();
        }


    }
}

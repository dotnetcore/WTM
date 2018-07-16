using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:treecontainer", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class TreeContainerTagHelper : BaseElementTag
    {
        protected const string REQUIRED_ATTR_NAME = "items";
        public ModelExpression Items { get; set; }
        /// <summary>
        /// 加载页面之前执行
        /// </summary>
        public string BeforeLoadEvent { get; set; }

        /// <summary>
        /// 自动加载首节点
        /// </summary>
        public bool AutoLoad { get; set; }

        /// <summary>
        /// 默认加载的页面
        /// </summary>
        public string AutoLoadUrl { get; set; }
        /// <summary>
        /// 加载页面之后执行
        /// </summary>
        public string AfterLoadEvent { get; set; }

        private string GetFirstNodeUrl(List<TreeSelectListItem> nodes)
        {
            if (nodes == null || nodes.Count == 0)
            {
                return string.Empty;
            }

            var node = nodes.FirstOrDefault();
            if (node.Children != null && node.Children.Count > 0)
            {
                return GetFirstNodeUrl(node.Children);
            }
            else
            {
                return node.Url;
            }
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            Id = string.IsNullOrEmpty(Id) ? Guid.NewGuid().ToNoSplitString() : Id;

            if (Items.Model is List<TreeSelectListItem> mm)
            {
                if (AutoLoad && string.IsNullOrEmpty(AutoLoadUrl))
                {
                    AutoLoadUrl = GetFirstNodeUrl(mm);
                }

                var script = $@"
<div id=""div_{Id}"" style=""position:absolute;left:200px;right:0;top:0;bottom:0;z-index:998;width:auto;overflow:hidden;overflow-y:auto;box-sizing:border-box""></div>                
<script>
layui.tree({{
  elem: '#{Id}',nameField:'text',childrenField:'children',spreadField:'expended'
  ,click:function(a){{
  {(string.IsNullOrEmpty(BeforeLoadEvent) ? string.Empty : $"{BeforeLoadEvent}(a);")}
  if(typeof(a.url)==""string""&&a.url){{ff.LoadPage1(a.url,'div_{Id}');}}
  {(string.IsNullOrEmpty(AfterLoadEvent) ? string.Empty : $"{AfterLoadEvent}(a);")}
  }}
  ,nodes: {JsonConvert.SerializeObject(mm, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() })}
}});
{(string.IsNullOrEmpty(AutoLoadUrl) ? string.Empty : $"ff.LoadPage1('{AutoLoadUrl}','div_{Id}');")}
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

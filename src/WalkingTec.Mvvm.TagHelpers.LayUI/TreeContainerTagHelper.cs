using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:treecontainer", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.NormalOrSelfClosing)]
    public class TreeContainerTagHelper : BaseElementTag
    {
        protected const string REQUIRED_ATTR_NAME = "items";
        public ModelExpression Items { get; set; }
        /// <summary>
        /// 加载页面之前执行
        /// </summary>
        public string ClickFunc { get; set; }

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

        public bool ShowLine { get; set; } = true;

        public string Title { get; set; }

        //当嵌套Grid时使用，树会把点击节点的ID传递给绑定的IdField，比如Searcher.xxxId
        public ModelExpression IdField { get; set; }
        //当嵌套Grid时使用，树会把点击节点的层级（从0开始的整形）传递给绑定的LevelField，比如Searcher.level
        public ModelExpression LevelField { get; set; }


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
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            Id = string.IsNullOrEmpty(Id) ? Guid.NewGuid().ToNoSplitString() : Id;
            output.Attributes.Add("id", "top" + Id);
            output.Attributes.Add("class", "layui-row donotuse_fill");
            if (Items.Model is List<TreeSelectListItem> mm)
            {
                if (AutoLoad && string.IsNullOrEmpty(AutoLoadUrl))
                {
                    AutoLoadUrl = GetFirstNodeUrl(mm);
                }
                var inside = await output.GetChildContentAsync();
                var insideContent = inside.GetContent();
                string cusmtomclick = "";
                if (string.IsNullOrEmpty(ClickFunc))
                {
                    Regex r = new Regex("(.*?)option = {");
                    var m = r.Match(insideContent);
                    if (m.Success)
                    {
                        var gridid = m.Groups[1].Value.Trim();
                        Regex r2 = new Regex($"(.*?) = table.render\\({gridid}option\\);");
                        var m2 = r2.Match(insideContent);
                        if (m2.Success)
                        {
                            var gridvar = m2.Groups[1].Value.Trim();
                            cusmtomclick = $@"
    layui.table.reload('{gridid}',{{where: $.extend({gridvar}.config.where,{{'{IdField?.Name ?? "notsetid"}':data.data.id, '{LevelField?.Name ?? "notsetlevel"}':data.data.level }}),
        done: function(res,curr,count){{
            layer.close(msg);
            if(this.height == undefined){{
                var tab = $('#{gridid} + .layui-table-view');tab.css('overflow','hidden').addClass('donotuse_fill donotuse_pdiv');tab.children('.layui-table-box').addClass('donotuse_fill donotuse_pdiv').css('height','100px');tab.find('.layui-table-main').addClass('donotuse_fill');tab.find('.layui-table-header').css('min-height','40px');
                ff.triggerResize();
            }}
        }}
    }})
";
                        }
                    }
                    else if(string.IsNullOrEmpty(insideContent))
                    {
                        cusmtomclick = $"if(data.data.href!=null && data.data.href!=''){{ff.LoadPage1(data.data.href,'div_{Id}');}}";
                    }
                }
                else
                {
                    cusmtomclick = $"{FormatFuncName(ClickFunc)};";
                }
                List<LayuiTreeItem> treeitems = GetLayuiTree(mm);
                var onclick = $@"
                ,click: function(data){{
                    var ele = data.elem.find('.layui-tree-entry:first');
                    if(last{Id} != null){{
                        last{Id}.css('background-color','');
                        last{Id}.find('.layui-tree-txt').css('color','');
                    }}
                    if(last{Id} === ele){{
                        last{Id} = null;
                    }}
                    else{{
                        ele.css('background-color','#5fb878');
                        ele.find('.layui-tree-txt').css('color','#fff');
                        last{Id} = ele;
                    }}
                    {cusmtomclick}
                  }}";

                var script = $@"
<div id=""div{Id}"" class=""layui-col-md2 donotuse_pdiv"" style=""padding-right:10px;border-right:solid 1px #aaa;""></div>
<div id=""div_{Id}"" style=""overflow:auto;box-sizing:border-box"" class=""layui-col-md10 donotuse_pdiv"">{insideContent}</div>
<script>
layui.use(['tree'],function(){{
  var last{Id} = null;
  layui.tree.render({{
    id:'tree{Id}',elem: '#div{Id}',onlyIconControl:true, showCheckbox:false,showLine:{ShowLine.ToString().ToLower()}
    {onclick}
    ,data: {JsonConvert.SerializeObject(treeitems)}
  }});
  {(string.IsNullOrEmpty(AutoLoadUrl) ? string.Empty : $"ff.LoadPage1('{AutoLoadUrl}','div_{Id}');")}
}})
</script>
";
                output.Content.SetHtmlContent(script);
            }
            else
            {
                output.Content.SetContent("无法绑定Tree，items参数必须设定为类型为List<ITreeData<>>的值");
            }
            base.Process(context, output);
        }

        private List<LayuiTreeItem> GetLayuiTree(List<TreeSelectListItem> tree, int level = 0)
        {
            List<LayuiTreeItem> rv = new List<LayuiTreeItem>();
            foreach (var s in tree)
            {
                var news = new LayuiTreeItem
                {
                    Id = s.Id,
                    Title = s.Text,
                    Url = s.Url,
                    Expand = s.Expended,
                    Level = level
                    //Children = new List<LayuiTreeItem>()
                };
                if (s.Children != null && s.Children.Count > 0)
                {
                    news.Children = GetLayuiTree(s.Children,level+1);
                    if (news.Children.Where(x => x.Checked == true || x.Expand == true).Count() > 0)
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

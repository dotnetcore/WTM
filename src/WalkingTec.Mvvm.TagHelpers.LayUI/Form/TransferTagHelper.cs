using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    /// <summary>
    /// 穿梭框
    /// </summary>
    [HtmlTargetElement("wt:transfer", Attributes = REQUIRED_ATTR_NAMES, TagStructure = TagStructure.WithoutEndTag)]
    public class TransferTagHelper : BaseFieldTag
    {
        private const string REQUIRED_ATTR_NAMES = "field";

        /// <summary>
        /// 左侧穿梭框上方标题
        /// </summary>
        public string LeftTitle { get; set; }

        /// <summary>
        /// 右侧穿梭框上方标题
        /// </summary>
        public string RightTitle { get; set; }

        /// <summary>
        /// 左侧穿梭框数据源
        /// </summary>
        public ModelExpression Items { get; set; }

        /// <summary>
        /// 启用搜索
        /// 默认 false
        /// </summary>
        public bool EnableSearch { get; set; }

        /// <summary>
        /// 没有数据时的文案
        /// </summary>
        public string NonePlaceholder { get; set; } = THProgram._localizer["Sys.NoData"];

        /// <summary>
        /// 搜索无匹配数据时的文案
        /// </summary>
        public string SearchNonePlaceholder { get; set; } = THProgram._localizer["Sys.NoMatchingData"];

        /// <summary>
        /// 当数据在左右穿梭时触发，回调返回当前被穿梭的数据
        /// param0: 得到当前被穿梭的数据
        /// param1: 如果数据来自左边，index 为 0，否则为 1
        /// param2: 当前 transfer 实例
        /// </summary>
        public string ChangeFunc { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("id", $"{Id}");
            output.Attributes.Add("wtm-ctype", "transfer");
            output.Attributes.Add("wtm-name", Field.Name);
            List<ComboSelectListItem> listItems = null;

            #region 添加下拉数据 并 设置默认选中

            var modeltype = Field.Metadata.ModelType;
            if (Items != null)
            {
                if (typeof(IEnumerable<ComboSelectListItem>).IsAssignableFrom(Items.Metadata.ModelType))
                {
                    if (typeof(IEnumerable<TreeSelectListItem>).IsAssignableFrom(Items.Metadata.ModelType))
                    {
                        listItems = (Items.Model as IEnumerable<TreeSelectListItem>)?.FlatTreeSelectList().Cast<ComboSelectListItem>().ToList();
                    }
                    else
                    {
                        listItems = (Items.Model as IEnumerable<ComboSelectListItem>)?.ToList();
                    }
                }

                if (listItems == null)
                {
                    listItems = new List<ComboSelectListItem>();
                    if (Items.Metadata.ModelType.IsList())
                    {
                        var exports = (Items.Model as IList);
                        if (exports != null)
                        {
                            foreach (var item in exports)
                            {
                                listItems.Add(new ComboSelectListItem
                                {
                                    Text = item?.ToString(),
                                    Value = item?.ToString()
                                });
                            }
                        }
                    }
                }
            }

            if(listItems == null)
            {
                listItems = new List<ComboSelectListItem>();
            }
            var data = listItems.Select(x => new
            {
                x.Value,
                Title = x.Text,
                x.Disabled,
                Checked = x.Selected
            }).ToArray();

            #endregion
            var selectVal = new List<string>();
            if (Field.Name.Contains("["))
            {
                //默认多对多不必填
                if (Required == null)
                {
                    Required = false;
                }
                if (Field?.ModelExplorer?.Container?.Model != null)
                {
                    selectVal.AddRange(Field.ModelExplorer.Container.Model.GetPropertySiblingValues(Field.Name));
                }
            }

            // 赋默认值
            else if (Field.Model != null)
            {
                if (modeltype.IsArray || (modeltype.IsGenericType && typeof(List<>).IsAssignableFrom(modeltype.GetGenericTypeDefinition())))
                {
                    foreach (var item in Field.Model as dynamic)
                    {
                        selectVal.Add($"{item.ToString()}");
                    }
                }
                else
                {
                    selectVal.Add(Field.Model.ToString());
                }
            }
            if(selectVal.Count > 0)
            {
                DefaultValue = $"[{string.Join(",", selectVal.Select(x => "'" + x + "'"))}]";
            }
            else
            {
                if(string.IsNullOrEmpty(DefaultValue) == false)
                {
                    DefaultValue = $"[{string.Join(",", DefaultValue.Split(",").Select(x => "'" + x + "'"))}]";
                }
            }
            var title = $"['{(string.IsNullOrEmpty(LeftTitle) ? THProgram._localizer["Sys.ForSelect"] : LeftTitle)}','{(string.IsNullOrEmpty(RightTitle) ? THProgram._localizer["Sys.Selected"] : RightTitle)}']";
            var content = $@"
<script>
layui.use(['transfer'],function(){{
  var $ = layui.$;
  var transfer = layui.transfer;
  var name = '{Field.Name}';
  var _id = '{Id}';
  var container = $('#'+_id+""div"");
  function defaultFunc(data,index,transferIns) {{
    var selectVals = transfer.getData('{Id}');
    /* remove old values */
    var inputs = $('#'+_id+'div input[name=""'+name+'""]')
    if(inputs!=null && inputs.length>0){{
      for (var i = 0; i < inputs.length; i++) {{
        inputs[i].remove();
      }}
    }}
    /* add new values */
    for (var i = 0; i < selectVals.length; i++) {{
      container.append('<input type=""hidden"" name=""'+name+'"" value=""'+selectVals[i].value+'""/>');
    }}
  }}
  var defaultVal = {(string.IsNullOrEmpty(DefaultValue) ? "[]" : DefaultValue)};
  var transferIns = transfer.render({{
    elem: '#'+_id
    ,title:{title}
    ,data:{JsonSerializer.Serialize(data,new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase})}
    {(string.IsNullOrEmpty(DefaultValue) ? string.Empty : $",value:defaultVal")}
    ,id:'{Id}'
    ,text:{{none:'{NonePlaceholder}',searchNone:'{SearchNonePlaceholder}'}}
    {(!EnableSearch ? string.Empty : ",showSearch:true")}
    {(!Width.HasValue ? string.Empty : $",width:{Width}")}
    {(!Height.HasValue ? string.Empty : $",height:{Height}")}
    ,onchange: function(data,index){{defaultFunc(data,index,transferIns);
    {(string.IsNullOrEmpty(ChangeFunc) ? string.Empty : $"{ChangeFunc}(data, index,transferIns);")}
    }}
  }});
  /* init default value */
  if(defaultVal!=null && defaultVal.length>0){{
    for (var i = 0; i < defaultVal.length; i++) {{
      container.append('<input type=""hidden"" name=""'+name+'"" value=""'+defaultVal[i]+'""/>');
    }}
  }}
  {(!Disabled?string.Empty: $@"
    $('#'+_id).find(':checkbox').prop('disabled',true)
    $('#'+_id).find(':input').prop('disabled',true)
    transfer.render();")}
}})
</script>
";
            output.PostElement.AppendHtml(content);
            output.PostElement.AppendHtml($"<div id=\"{Id}div\"></div>");
            output.PostElement.AppendHtml($@"<input type=""hidden"" name=""_DONOTUSE_{Field.Name}"" value=""1"" />");
            if (string.IsNullOrEmpty(ItemUrl) == false)
            {
                output.PostElement.AppendHtml($"<script>ff.LoadComboItems('transfer','{ItemUrl}','{Id}','{Field.Name}',{JsonSerializer.Serialize(selectVal)})</script>");
            }

            base.Process(context, output);
        }
    }
}

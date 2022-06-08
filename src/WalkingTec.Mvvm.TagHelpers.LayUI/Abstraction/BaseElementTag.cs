using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public abstract class BaseElementTag : TagHelper
    {
        public int? Colspan { get; set; }
        public string Id { get; set; }

        public int? Height { get; set; }

        public int? Width { get; set; }

        public string Class { get; set; }

        public string Style { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var preHtml = string.Empty;
            var postHtml = string.Empty;
            if (output.Attributes.ContainsName("id") == false && string.IsNullOrEmpty(Id) == false)
            {
                output.Attributes.SetAttribute("id", Id);
            }
            if (output.Attributes.ContainsName("lay-filter") == false && output.Attributes.ContainsName("id") == true)
            {
                output.Attributes.SetAttribute("lay-filter", $"{output.Attributes["id"].Value}filter");
            }
            if (string.IsNullOrEmpty(Class) == false)
            {
                output.Attributes.SetAttribute("class", Class);
            }
            if (Style == null)
            {
                Style = "";
            }
            if (Width.HasValue)
            {
                Style += $" width:{Width}px;";
            }
            if (Height.HasValue)
            {
                Style += $" min-height:{Height}px;";
            }
            if (string.IsNullOrEmpty(Style) == false)
            {
                if (this is TreeTagHelper )
                {
                    Style += " overflow:auto;";
                }
                TagHelperAttribute prestyle = null;
                if(output.Attributes.TryGetAttribute("style", out prestyle))
                {
                    string s = prestyle.Value.ToString();
                    if(s.EndsWith(";") == false)
                    {
                        s += ";";
                    }
                    Style = s+Style;
                }
                
                output.Attributes.SetAttribute("style",  Style);
            }

            if (context.Items.ContainsKey("ipr"))
            {
                int? ipr = (int?)context.Items["ipr"];
                if (ipr > 0)
                {
                    int col = 12 / ipr.Value;
                    if (Colspan != null)
                    {
                        col *= Colspan.Value;
                    }
                    preHtml = $@"
<div class=""layui-col-md{col}"">
" + preHtml;
                    postHtml += @"
</div>
";
                    output.PreElement.SetHtmlContent(preHtml + output.PreElement.GetContent());
                    output.PostElement.AppendHtml(postHtml);
                }
                if(this is CardTagHelper || this is FormTagHelper || this is ContainerTagHelper || this is TreeContainerTagHelper || this is SearchPanelTagHelper)
                {
                    context.Items.Remove("ipr");
                }
            }
            //输出事件
            switch (this)
            {
                case ComboBoxTagHelper item:
                    if(item.MultiSelect == true)
                    {
                        break;
                    }
//                    if (item.LinkField != null || item.LinkId != null)
//                    {
//                        if (!string.IsNullOrEmpty(item.TriggerUrl))
//                        {
//                            output.PostElement.AppendHtml($@"
//<script>
//layui.use(['form'],function(){{
//  var form = layui.form;
//  form.on('select({output.Attributes["lay-filter"].Value})', function(data){{
//    {FormatFuncName(item.ChangeFunc)};
//    ff.ChainChange('{item.TriggerUrl}/'+data.value,data.elem)
//    ff.changeComboIcon(data);
//  }});
//}})
//</script>
//");
//                        }
//                    }
//                    else
//                    {
//                        output.PostElement.AppendHtml($@"
//<script>
//layui.use(['form'],function(){{
//  var form = layui.form;
//  form.on('select({output.Attributes["lay-filter"].Value})', function(data){{
//    {FormatFuncName(item.ChangeFunc)};
//    ff.changeComboIcon(data);
//  }});
//}})
//</script>
//");
//                    }
                    break;
                case CheckBoxTagHelper item:
                    if (string.IsNullOrEmpty(item.ChangeFunc) == false)
                    {
                        output.PostContent.SetHtmlContent(output.PostContent.GetContent().Replace("type=\"checkbox\" ", $"type=\"checkbox\" lay-filter=\"{output.Attributes["lay-filter"].Value}\""));
                        output.PostElement.AppendHtml($@"
<script>
layui.use(['form'],function(){{
  var form = layui.form;
  form.on('checkbox({output.Attributes["lay-filter"].Value})', function(data){{
    {FormatFuncName(item.ChangeFunc)};
  }});
}})
</script>
");
                    }
                    break;
                case SwitchTagHelper item:
                    if (string.IsNullOrEmpty(item.ChangeFunc) == false)
                    {
                        output.PostElement.AppendHtml($@"
<script>
layui.use(['form'],function(){{
  var form = layui.form;
  form.on('switch({output.Attributes["lay-filter"].Value})', function(data){{
    {FormatFuncName(item.ChangeFunc)};
  }});
}})
</script>
");
                    }
                    break;
                case RadioTagHelper item:
                    if (string.IsNullOrEmpty(item.ChangeFunc) == false)
                    {
                        output.PostContent.SetHtmlContent(output.PostContent.GetContent().Replace("type=\"radio\" ", $"type=\"radio\" lay-filter=\"{output.Attributes["lay-filter"].Value}\""));
                        output.PostElement.AppendHtml($@"
<script>
layui.use(['form'],function(){{
  var form = layui.form;
  form.on('radio({output.Attributes["lay-filter"].Value})', function(data){{
    {FormatFuncName(item.ChangeFunc)};
  }});
}})
</script>
");
                    }
                    break;
                case TextBoxTagHelper item:
                    if (string.IsNullOrEmpty(item.SearchUrl) == false)
                    {
                        if (!string.IsNullOrEmpty(item.TriggerUrl))
                        {
                            output.PostElement.AppendHtml($@"
<script>
layui.use(['autocomplete'],function(){{
  layui.autocomplete.render({{
    elem: $('#{item.Id}')[0],
    url: '{item.SearchUrl}',
    cache: false,
    template_val: '{{{{d.Value}}}}',
    template_txt: '{{{{d.Text}}}}',
    onselect: function (data) {{
      $('#{item.Id}').val(data.Value);
     {FormatFuncName(item.ChangeFunc)};
     ff.ChainChange('{item.TriggerUrl}/'+data.Value, data.elem);
    }}
  }});
}})
</script>
");
                        }
                        else
                        {
                            output.PostElement.AppendHtml($@"
<script>
layui.use(['autocomplete'],function(){{
  layui.autocomplete.render({{
    elem: $('#{item.Id}')[0],
    url: '{item.SearchUrl}',
    cache: false,
    template_val: '{{{{d.Value}}}}',
    template_txt: '{{{{d.Text}}}}',
    onselect: function (data) {{
      $('#{item.Id}').val(data.Value);
     {FormatFuncName(item.ChangeFunc)};
    }}
  }});
}})
</script>
");

                        }
                    }
                    break;
            }

            //如果是submitbutton，则在button前面加入一个区域用来定位输出后台返回的错误
            //if (output.TagName == "button" && output.Attributes.TryGetAttribute("lay-submit", out TagHelperAttribute ta) == true)
            //{
            //    output.PreElement.SetHtmlContent($"<p id='{Id}errorholder'></p>" + output.PreElement.GetContent());
            //}
        }

        public string FormatFuncName(string funcname,bool appendparameter = true)
        {
            if (funcname == null)
            {
                return null;
            }
            var rv = funcname;
            var ind = rv.IndexOf("(");
            if (ind > 0)
            {
                rv = rv.Substring(0, ind);
            }
            if (appendparameter == true)
            {
                rv += "(data)";
            }
            return rv;
        }
    }
}

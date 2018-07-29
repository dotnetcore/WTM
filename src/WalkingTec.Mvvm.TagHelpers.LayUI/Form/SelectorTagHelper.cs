using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI.Form
{
    [HtmlTargetElement("wt:selector", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.NormalOrSelfClosing)]
    public class SelectorTagHelper : BaseFieldTag
    {
        private new const string REQUIRED_ATTR_NAME = "field,list-vm,text-bind";

        /// <summary>
        /// EmptyText
        /// </summary>
        public string EmptyText { get; set; }

        /// <summary>
        /// 按钮文本
        /// </summary>
        public string SelectButtonText { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string WindowTitle { get; set; }

        /// <summary>
        /// 显示
        /// </summary>
        public bool Display { get; set; }

        /// <summary>
        /// 宽
        /// </summary>
        public int? WindowWidth { get; set; }

        /// <summary>
        /// 高
        /// </summary>
        public int? WindowHeight { get; set; }

        /// <summary>
        /// 是否多选 
        /// 默认根据Field 绑定的值类型进行判断。Array or List 即多选，否则单选
        /// </summary>
        public bool? MultiSelect { get; set; }

        /// <summary>
        /// ListVM
        /// </summary>
        public ModelExpression ListVM { get; set; }

        /// <summary>
        /// 文本显示字段
        /// </summary>
        public ModelExpression TextBind { get; set; }

        /// <summary>
        /// 暂时无用 默认 Id
        /// </summary>
        public ModelExpression ValBind { get; set; }

        public string SubmitFunc { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            #region Display Value

            var modelType = Field.Metadata.ModelType;
            var list = new List<Guid>();
            if (Field.Model != null)
            {
                // 数组 or 泛型集合
                if (modelType.IsArray || (modelType.IsGenericType && typeof(List<>).IsAssignableFrom(modelType.GetGenericTypeDefinition())))
                {
                    foreach (var item in Field.Model as dynamic)
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    list.Add(Guid.Parse(Field.Model.ToString()));
                }
            }

            var value = new List<string>();
            if (list.Count > 0)
            {
                if (ListVM == null || ListVM.Model == null)
                    throw new Exception("Selector 组件指定的 ListVM 必须要实例化");
                var listVM = ListVM.Model as IBasePagedListVM<TopBasePoco, ISearcher>;
                if (context.Items.ContainsKey("model") == true)
                {
                    listVM.CopyContext(context.Items["model"] as BaseVM);
                }
                listVM.Ids = list;
                listVM.NeedPage = false;
                listVM.IsSearched = false;
                listVM.ClearEntityList();
                listVM.SearcherMode = ListVMSearchModeEnum.Batch;
                var entityList = listVM.GetEntityList().ToList();
                foreach (var item in entityList)
                {
                    value.Add(item.GetType().GetProperty(TextBind?.Metadata.PropertyName)?.GetValue(item).ToString());
                }
            }

            #endregion

            if (Display)
            {
                output.TagName = "label";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.Add("class", "layui-form-label");
                output.Attributes.Add("style", "text-align:left;padding:9px 0;");
                var val = string.Empty;
                if (Field.Model != null)
                {
                    if (Field.Model.GetType().IsEnumOrNullableEnum())
                    {
                        val = PropertyHelper.GetEnumDisplayName(Field.Model.GetType(), Field.Model.ToString());
                    }
                    else
                    {
                        val = Field.Model.ToString();
                    }
                }
                output.Content.AppendHtml(string.Join(",", value));

                base.Process(context, output);
            }
            else
            {

                var windowid = Guid.NewGuid().ToString();
                if (MultiSelect == null)
                {
                    MultiSelect = false;
                    var type = Field.Metadata.ModelType;
                    if (type.IsArray || (type.IsGenericType && typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition())))// Array or List
                    {
                        MultiSelect = true;
                    }
                }
                if (WindowWidth == null)
                {
                    WindowWidth = 800;
                }

                output.TagName = "input";
                output.TagMode = TagMode.StartTagOnly;
                output.Attributes.Add("type", "text");
                output.Attributes.Add("id", Id + "_Display");
                output.Attributes.Add("name", Field.Name + "_Display");

                var content = output.GetChildContentAsync().Result.GetContent().Trim();

                #region 移除因 RowTagHelper 生成的外层 div 即 <div class="layui-col-xs6"></div>

                var regStart = new Regex(@"^<div\s+class=""layui-col-xs[0-9]+"">");
                var regEnd = new Regex("</div>$");
                content = regStart.Replace(content, string.Empty);
                content = regEnd.Replace(content, string.Empty);

                #endregion

                var reg = new Regex("(name=\")([0-9a-zA-z]{0,}[.]?)(Searcher[.]?[0-9a-zA-z]{0,}\")", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                content = reg.Replace(content, "$1$3");
                content = content.Replace("<script>", "$$script$$").Replace("</script>", "$$#script$$");
                var searchPanelTemplate = $@"<script type=""text/template"" id=""Temp{Id}"">{content}</script>";

                output.Attributes.Add("value", string.Join(",", value));
                output.Attributes.Add("placeholder", EmptyText ?? "请选择");
                output.Attributes.Add("class", "layui-input");
                this.Disabled = true;
                var vmQualifiedName = ListVM.Metadata.ModelType.AssemblyQualifiedName;
                vmQualifiedName = vmQualifiedName.Substring(0, vmQualifiedName.LastIndexOf(", Version="));
                var Filter = new Dictionary<string, object>
            {
                { "_DONOT_USE_VMNAME", vmQualifiedName },
                { "_DONOT_USE_KFIELD", TextBind?.Metadata.PropertyName },
                { "_DONOT_USE_VFIELD", ValBind == null ? "ID" : ValBind?.Metadata.PropertyName },
                { "_DONOT_USE_FIELD", Field.Name },
                { "_DONOT_USE_MULTI_SEL", MultiSelect },
                { "_DONOT_USE_SEL_ID", Id },
                { "Ids", list }
            };
                if (!string.IsNullOrEmpty(SubmitFunc))
                {
                    Filter.Add("_DONOT_USE_SUBMIT", SubmitFunc);
                }
                var hiddenStr = string.Empty;
                var sb = new StringBuilder();
                foreach (var item in list)
                {
                    sb.Append($"<input type='hidden' name='{Field.Name}' value='{item.ToString()}' />");
                }
                hiddenStr = sb.ToString();
                output.PreElement.AppendHtml($@"<div id=""{Id}_Container"" style=""position:absolute;right:50px;left:0px;width:auto"">");
                output.PostElement.AppendHtml($@"
{hiddenStr}
</div>
<button class='layui-btn layui-btn-sm layui-btn-warm' type='button' id='{Id}_Select' style='color:white;position:absolute;right:0px'>{SelectButtonText ?? "选择"}</button>
<hidden id='{Id}' name='{Field.Name}' />
<script>
$('#{Id}_Select').on('click',function(){{
    var filter = {JsonConvert.SerializeObject(Filter)};
    var vals = $('#{Id}_Container input[type=hidden]');
    filter.Ids = []
    for(var i=0;i<vals.length;i++){{
        filter.Ids.push(vals[i].value);
    }}
    ff.OpenDialog2('/_Framework/Selector', '{windowid}', '{WindowTitle ?? string.Empty}',{WindowWidth?.ToString() ?? "null"}, {WindowHeight?.ToString() ?? "null"},'#Temp{Id}', filter);
}});
</script>
{searchPanelTemplate}
");
                base.Process(context, output);
            }
        }
    }
}

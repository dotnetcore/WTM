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

        public ModelExpression LinkField { get; set; }

        public string TriggerUrl { get; set; }

        /// <summary>
        /// 按钮文本
        /// </summary>
        public string SelectButtonText { get; set; }

        /// <summary>
        /// 选择按钮宽度，默认50
        /// </summary>
        public int? SelectButtonWidth { get; set; }
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

        /// <summary>
        /// 用于指定弹出窗口的搜索条件，多条件逗号分隔，例如Searcher.Con1=a,Searcher.Con2=b
        /// </summary>
        public string Paras { get; set; }

        public string SubmitFunc { get; set; }

        /// <summary>
        /// 弹出窗口之前运行的js函数
        /// </summary>
        public string BeforeOnpenDialogFunc { get; set; }


        /// <summary>
        /// 排除的搜索条件
        /// </summary>
        private static readonly string[] _excludeParams = new string[]
        {
            "Page",
            "Limit",
            "Count",
            "PageCount",
            "FC",
            "DC",
            "VMFullName",
            "SortInfo",
            "TreeMode",
            "IsPostBack",
            "DC",
            "LoginUserInfo"
        };

        /// <summary>
        /// 排除的搜索条件类型，搜索条件数据源可能会存储在Searcher对象中
        /// </summary>
        private static readonly Type[] _excludeTypes = new Type[]
        {
            typeof(List<ComboSelectListItem>),
            typeof(ComboSelectListItem[]),
            typeof(IEnumerable<ComboSelectListItem>)
        };

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            #region Display Value

            var modelType = Field.Metadata.ModelType;
            var list = new List<string>();
            if (Field.Model != null)
            {
                // 数组 or 泛型集合
                if (modelType.IsArray || (modelType.IsGenericType && typeof(List<>).IsAssignableFrom(modelType.GetGenericTypeDefinition())))
                {
                    foreach (var item in Field.Model as dynamic)
                    {
                        list.Add(item.ToString());
                    }
                }
                else
                {
                    list.Add(Field.Model.ToString());
                }
            }
            if (ListVM == null || ListVM.Model == null)
                throw new Exception("The ListVM of the Selector is null");
            var listVM = ListVM.Model as IBasePagedListVM<TopBasePoco, ISearcher>;
            var value = new List<string>();
            if (context.Items.ContainsKey("model") == true)
            {
                listVM.CopyContext(context.Items["model"] as BaseVM);
            }
            if (list.Count > 0)
            {
                listVM.Ids = list;
                listVM.NeedPage = false;
                listVM.IsSearched = false;
                listVM.ClearEntityList();
                listVM.SearcherMode = ListVMSearchModeEnum.Batch;
                if (!string.IsNullOrEmpty(Paras))
                {
                    var p = Paras.Split(',');
                    Regex r = new Regex("(\\s*)\\S*?=\\S*?(\\s*)");
                    foreach (var item in p)
                    {
                        var s = Regex.Split(item, "=");
                        if (s != null && s.Length == 2)
                        {
                            listVM.SetPropertyValue(s[0], s[1]);
                        }

                    }
                }

                var entityList = listVM.GetEntityList().ToList();
                foreach (var item in entityList)
                {
                    value.Add(item.GetType().GetSingleProperty(TextBind?.Metadata.PropertyName)?.GetValue(item).ToString());
                }
            }

            #endregion

            if (Display)
            {
                output.TagName = "label";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.Add("class", "layui-form-label");
                output.Attributes.Add("style", "text-align:left;padding:9px 0;width:unset;");
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

                if (listVM.Searcher != null)
                {
                    var searcher = listVM.Searcher;
                    searcher.CopyContext(listVM);
                    searcher.DoInit();
                }
                var content = output.GetChildContentAsync().Result.GetContent().Trim();

                #region 移除因 RowTagHelper 生成的外层 div 即 <div class="layui-col-xs6"></div>

                var regStart = new Regex(@"^<div\s+class=""layui-col-md[0-9]+"">");
                var regEnd = new Regex("</div>$");
                content = regStart.Replace(content, string.Empty);
                content = regEnd.Replace(content, string.Empty);

                #endregion

                var reg = new Regex("(name=\")([0-9a-zA-z]{0,}[.]?)(Searcher[.]?[0-9a-zA-z]{0,}\")", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                content = reg.Replace(content, "$1$3");
                content = content.Replace("<script>", "$$script$$").Replace("</script>", "$$#script$$");
                var searchPanelTemplate = $@"<script type=""text/template"" id=""Temp{Id}"">{content}</script>";

                output.Attributes.Add("value", string.Join(",", value));
                output.Attributes.Add("placeholder", EmptyText ?? Program._localizer["PleaseSelect"]);
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
                if (!string.IsNullOrEmpty(TriggerUrl) && LinkField != null)
                {
                    Filter.Add("_DONOT_USE_LINK_FIELD_MODEL", LinkField.ModelExplorer.Container.ModelType.Name + "." + LinkField.Name);
                    Filter.Add("_DONOT_USE_LINK_FIELD", LinkField.Name);
                    Filter.Add("_DONOT_USE_TRIGGER_URL", TriggerUrl);
                }
                if (listVM.Searcher != null)
                {
                    var props = listVM.Searcher.GetType().GetProperties();
                    props = props.Where(x => !_excludeTypes.Contains(x.PropertyType)).ToArray();
                    foreach (var prop in props)
                    {
                        if (!_excludeParams.Contains(prop.Name))
                        {
                            Filter.Add($"Searcher.{prop.Name}", prop.GetValue(listVM.Searcher));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Paras))
                {
                    var p = Paras.Split(',');
                    Regex r = new Regex("(\\s*)\\S*?=\\S*?(\\s*)");
                    foreach (var item in p)
                    {
                        var s = Regex.Split(item, "=");
                        if (s != null && s.Length == 2)
                        {
                            if (Filter.ContainsKey(s[0]))
                            {
                                Filter[s[0]] = s[1];
                            }
                            else
                            {
                                Filter.Add(s[0], s[1]);
                            }
                        }

                    }
                }
                var hiddenStr = string.Empty;
                var sb = new StringBuilder();
                foreach (var item in list)
                {
                    sb.Append($"<input type='hidden' name='{Field.Name}' value='{item.ToString()}' />");
                }
                hiddenStr = sb.ToString();
                output.PreElement.AppendHtml($@"<div id=""{Id}_Container"" style=""position:absolute;right:{SelectButtonWidth?.ToString() ?? "50"}px;left:0px;width:auto"">");
                output.PostElement.AppendHtml($@"
{hiddenStr}
</div>
<button class='layui-btn layui-btn-sm layui-btn-warm' type='button' id='{Id}_Select' style='color:white;position:absolute;right:0px'>{SelectButtonText ?? " . . . "}</button>
<hidden id='{Id}' name='{Field.Name}' />
<script>
var {Id}filter = {{}};
$('#{Id}_Select').on('click',function(){{
  {(string.IsNullOrEmpty(BeforeOnpenDialogFunc) == true ? "" : "var data={};" + FormatFuncName(BeforeOnpenDialogFunc) + ";")}
  var filter = {JsonConvert.SerializeObject(Filter)};
  var vals = $('#{Id}_Container input[type=hidden]');
  filter.Ids = [];
  for(var i=0;i<vals.length;i++){{
    filter.Ids.push(vals[i].value);
  }};
  var ffilter = $.extend(filter, {Id}filter)
  ff.OpenDialog2('/_Framework/Selector', '{windowid}', '{WindowTitle ?? Program._localizer["PleaseSelect"]}',{WindowWidth?.ToString() ?? "null"}, {WindowHeight?.ToString() ?? "500"},'#Temp{Id}', ffilter);
}});
</script>
{searchPanelTemplate}
");
                base.Process(context, output);
            }
        }
    }
}

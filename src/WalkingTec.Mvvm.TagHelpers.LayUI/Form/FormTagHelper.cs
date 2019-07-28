using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:form", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.NormalOrSelfClosing)]
    public class FormTagHelper : BaseElementTag
    {
        protected const string REQUIRED_ATTR_NAME = "vm";
        public const string FORM_ID_PREFIX = "wtForm_";

        private string _id = null;
        public new string Id
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    var vm = Vm.Model as IBaseVM;
                    _id = $"{FORM_ID_PREFIX}{vm.UniqueId}";
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// 提交表单的Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 表单绑定的Vm
        /// </summary>
        public ModelExpression Vm { get; set; }

        /// <summary>
        /// 提交前调用的js
        /// </summary>
        public string BeforeSubmit { get; set; }

        /// <summary>
        /// 使用传统的提交整个页面的方式，而不使用ajax提交
        /// </summary>
        public bool OldPost { get; set; }

        /// <summary>
        /// 设置表单内控件前面label的长度，默认为80
        /// </summary>
        public int? LabelWidth { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var baseVM = Vm?.Model as BaseVM;
            if (baseVM == null)
            {
                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.SetContent("无法获得页面关联的VM，请检查<wt:form>是否指定了Vm属性");
                return;
            }
            output.TagName = "form";
            output.Attributes.SetAttribute("id", Id);
            output.Attributes.SetAttribute("class", "layui-form");
            if (!(this is SearchPanelTagHelper))
            {
                output.Attributes.SetAttribute("style", "margin:10px");
                //添加items以便子项可以使用
                if (context.Items.ContainsKey("formid") == false)
                {
                    context.Items.Add("formid", Id);
                }
            }
            output.Attributes.SetAttribute("method", "post");
            output.Attributes.SetAttribute("lay-filter", Id + "form");
            if (LabelWidth != null)
            {
                context.Items.Add("formlabelwidth", LabelWidth.Value);
            }
            if (context.Items.ContainsKey("model") == false)
            {
                context.Items.Add("model", baseVM);
            }

            if (string.IsNullOrEmpty(Url) == false)
            {
                output.Attributes.SetAttribute("action", Url);
            }
            else
            {
                //设置提交地址，如果不指定Url，默认为当前页面。如果绑定Vm为BatchVM，将提交地址由BatchXXX变为DoBatchXXX。
                //因为框架默认的Batch本身是一个Post方法，无法使用同名方法处理提交后的工作
                if (Vm.Model is IBaseBatchVM<BaseVM>)
                {
                    output.Attributes.SetAttribute("action", baseVM?.CurrentUrl.Replace("/Batch", "/DoBatch") ?? "#");
                }
                else
                {
                    output.Attributes.SetAttribute("action", baseVM?.CurrentUrl ?? "#");
                }
            }
            //await output.GetChildContentAsync();
            output.PostElement.AppendHtml($@"
<script>
  var form = layui.form.render(null,'{Id}form');
  var comboxs = $("".layui-form[lay-filter='{Id}form'] select[wtm-combo='MULTI_COMBO']"");
  /* 启用 ComboBox 多选 */
  for(var i=0;i<comboxs.length;i++){{
    var filter = comboxs[i].attributes['lay-filter'].value,arr = [],subTag = $('input[name=""' + filter + '""]');
    for (var i = 0; i < subTag.length; i++) {{
      arr.push({{name:subTag[i].attributes['text'].value,val:subTag[i].value}});
    }}
    formSelects.on({{
      layFilter:filter,left:'',right:'',separator:',',arr:arr,
      selectFunc: null
    }})
  }}
");
            //如果是普通表单并且没有设置oldpost，则使用ajax方式提交
            //如果普通表单设置了oldpost，则用传统form提交
            if (OldPost == false && !(this is SearchPanelTagHelper))
            {
                output.PostElement.AppendHtml($@"
  layui.form.on('submit({Id}filter)', function(data){{
    if({BeforeSubmit ?? "true"} == false){{return false;}}
    ff.PostForm('{output.Attributes["action"].Value}', '{Id}', '{baseVM?.ViewDivId}')
    return false;
  }});
");
            }
            //如果是SearchPanel，并且指定了oldpost，则提交整个表单，而不是只刷新grid数据
            if( OldPost == true && this is SearchPanelTagHelper search)
            {
                output.PostElement.AppendHtml($@"
$('#{search.SearchBtnId}').on('click', function () {{
    if({BeforeSubmit ?? "true"} == false){{return false;}}
    ff.PostForm('{output.Attributes["action"].Value}', '{Id}', '{baseVM?.ViewDivId}')
    return false;
  }});
");

            }
            //输出后台返回的错误信息
            if (baseVM?.MSD?.Count > 0)
            {
                string firstkey = null;
                foreach (var key in baseVM.MSD.Keys)
                {
                    bool haserror = false;
                    foreach (var error in baseVM.MSD[key])
                    {
                        haserror = true;
                        if (firstkey == null)
                        {
                            firstkey = key;
                        }
                        output.PostElement.AppendHtml($@"
$(""#{Id}submiterrorholder"").before(""<div class='layui-input-block' style='text-align:left'><label style='color:red'>{error.ErrorMessage}</label></div>"");
");
                    }
                    if (haserror == true)
                    {
                        output.PostElement.AppendHtml($@"$(""#{Utils.GetIdByName(baseVM.GetType().Name+"."+key)}"").addClass('layui-form-danger');");
                    }
                }
                if (firstkey != null)
                {
                    output.PostElement.AppendHtml($@"$(""#{Utils.GetIdByName(baseVM.GetType().Name + "."+firstkey)}"").focus();");
                }
            }
            output.PostElement.AppendHtml($@"</script>");
            base.Process(context, output);
        }
    }
}

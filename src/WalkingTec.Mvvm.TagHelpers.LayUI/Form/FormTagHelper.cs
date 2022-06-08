using System.Text.RegularExpressions;
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
                    if (Vm?.Model is IBaseVM)
                    {
                        var vm = Vm.Model as IBaseVM;
                        _id = $"{FORM_ID_PREFIX}{vm.UniqueId}";
                    }
                    else if(Vm?.Model is BaseSearcher)
                    {
                        var vm = Vm.Model as BaseSearcher;
                        _id = $"{FORM_ID_PREFIX}{vm.UniqueId}";
                    }
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
        /// 使用传统表单提交方式提交，而不使用 AJAX 提交
        /// 比如登陆页面，提交后校验成功会跳转其他页面，而不是返会 PartialView
        /// </summary>
        public bool OldPost { get; set; }

        /// <summary>
        /// 设置表单内控件前面label的长度，默认为80
        /// </summary>
        public int? LabelWidth { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var novm = true;
            BaseVM baseVM = null;
            BaseSearcher baseSearcher = null;
            if (Vm?.Model is  BaseVM)
            {
                novm = false;
                baseVM = Vm?.Model as BaseVM;
            }
            if(Vm?.Model is BaseSearcher )
            {
                novm = false;
                baseSearcher = Vm?.Model as BaseSearcher;
            }
            if (novm)
            {
                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.SetContent("VM is not set, please check if <wt:form> set the vm field");
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
            output.Attributes.SetAttribute("lay-filter", Id);
            if (LabelWidth != null)
            {
                if (context.Items.ContainsKey("formlabelwidth") == false)
                {
                    context.Items.Add("formlabelwidth", LabelWidth.Value);
                }
            }
            if (context.Items.ContainsKey("model") == false)
            {
                if(baseVM != null)
                {
                    context.Items.Add("model", baseVM);
                }
                else if(baseSearcher != null)
                {
                    context.Items.Add("model", baseSearcher);
                }
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
                    output.Attributes.SetAttribute("action", baseVM?.CurrentUrl ?? baseSearcher?.Wtm?.BaseUrl ?? "#");
                }
            }
            output.PostContent.AppendHtml($"<input type='hidden' name='FromView' value='{baseVM?.CurrentView}' />");

            output.PostElement.AppendHtml($@"
<script>
ff.RenderForm('{Id}');
");

            if(BeforeSubmit != null && BeforeSubmit.Contains("(") == false)
            {
                BeforeSubmit += "()";
            }
            // 使用传统表单提交方式提交，而不使用 AJAX 提交
            // 比如登陆页面，提交后校验成功会跳转其他页面，而不是返会 PartialView
            if (OldPost == false && !(this is SearchPanelTagHelper))
            {
                output.PostElement.AppendHtml($@"
layui.use(['form'],function(){{
  layui.form.on('submit({Id}filter)', function(data){{
    if({BeforeSubmit ?? "true"} == false){{return false;}}
    ff.PostForm('', '{Id}', '{baseVM?.ViewDivId}')
    return false;
  }});
}})
");
                output.PostElement.AppendHtml($@"
var {Id}validate = false;
layui.use(['form'],function(){{
  layui.form.on('submit({Id}filterAuto)', function(data){{
  {Id}validate = true;
  return false;
  }});
}})
");
                output.PostContent.AppendHtml($@"
<button class=""layui-hide"" id=""{Id}hidesubmit""  type=""submit"" lay-filter=""{Id}filterAuto"" lay-submit></button>
");

            }

            //如果是 SearchPanel，并且指定了 OldPost，则提交整个表单，而不是只刷新 Grid 数据
            if (OldPost == true && this is SearchPanelTagHelper search)
            {
                output.PostElement.AppendHtml($@"
$('#{search.SearchBtnId}').on('click', function () {{
    if({BeforeSubmit ?? "true"} == false){{return false;}}
    ff.PostForm('', '{Id}', '{baseVM?.ViewDivId ?? baseSearcher?.ViewDivId}','{Vm?.Name}')
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
$(""#{Id}"").find(""button[type=submit]:first"").parent().prepend(""<div class='layui-input-block' style='text-align:left'><label style='color:red'>{Regex.Replace(error.ErrorMessage,"<script>","", RegexOptions.IgnoreCase)}</label></div>"");
");
                    }
                    if (haserror == true)
                    {
                        output.PostElement.AppendHtml($@"$(""#{Utils.GetIdByName(baseVM.GetType().Name + "." + key)}"").addClass('layui-form-danger');");
                    }
                }
                if (firstkey != null)
                {
                    output.PostElement.AppendHtml($@"$(""#{Utils.GetIdByName(baseVM.GetType().Name + "." + firstkey)}"").focus();");
                }
            }
            output.PostElement.AppendHtml($@"</script>");
            base.Process(context, output);
        }
    }
}

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public abstract class BaseFieldTag : BaseElementTag
    {
        protected const string REQUIRED_ATTR_NAME = "field";
        /// <summary>
        /// 绑定的字段 必填
        /// </summary>
        public ModelExpression Field { get; set; }

        public bool Disabled { get; set; }

        public string Name { get; set; }

        public string LabelText { get; set; }

        public int? LabelWidth { get; set; }

        public bool HideLabel { get; set; }
        private string _id;
        public new string Id
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    string rv = string.Empty;
                    if(Field != null)
                    {
                        rv = Utils.GetIdByName(Field?.ModelExplorer.Container.ModelType.Name + "." + Field?.Name);
                    }
                    return  rv;
                }
                else
                {
                    return _id;
                }
            }
            set
            {
                _id = value;
            }
        }

        public string PaddingText { get; set; }

        public string DefaultValue { get; set; }

        /// <summary>
        /// 不需要生成必填验证
        /// </summary>
        private static readonly string[] _excludeType = new string[]
        {
            "checkbox",
            "radio",
            "select"
        };
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var preHtml = string.Empty;
            var postHtml = string.Empty;
            var requiredDot = string.Empty;
            if (output.Attributes.TryGetAttribute("id", out TagHelperAttribute a) == false)
            {
                output.Attributes.SetAttribute("id", Id ?? string.Empty);
            }
            if (output.Attributes.TryGetAttribute("name", out TagHelperAttribute b) == false)
            {
                output.Attributes.SetAttribute("name", string.IsNullOrEmpty(Name) ? Field?.Name : Name);
            }
            if (Disabled)
            {
                output.Attributes.SetAttribute("readonly", string.Empty);
                output.Attributes.TryGetAttribute("class", out TagHelperAttribute oldclass);
                output.Attributes.SetAttribute("class", "layui-disabled " + (oldclass?.Value ?? string.Empty));
            }

            if (!(this is DisplayTagHelper) && !(this is CheckBoxTagHelper) && Field.Metadata.IsRequired)
            {
                requiredDot = "<font color='red'>*</font>";
                if (!(this is UploadTagHelper)) // 上传组件自定义验证
                {
                    if (!_excludeType.Contains(output.Attributes["type"]?.Value?.ToString()))
                    {
                        //richtextbox不需要进行必填验证
                        if (output.Attributes["isrich"] == null)
                        {
                            output.Attributes.Add("lay-verify", "required");
                        }
                    }
                }
            }

            if (LabelText == null)
            {
                LabelText = Field?.Metadata.DisplayName ?? Field?.Metadata.PropertyName;
                if (LabelText == null)
                {
                    HideLabel = true;
                }
            }

            if (LabelWidth == null && context.Items.ContainsKey("formlabelwidth"))
            {
                LabelWidth = (int)context.Items["formlabelwidth"];
            }
            //如果不显示label则隐藏

            if (HideLabel == false)
            {
                string lb = "";
                if(LabelText != "") {
                    lb = $"{requiredDot}{LabelText}:";
                }

                preHtml += $@"
<div {(this is DisplayTagHelper ? "style=\"margin-bottom:0px;\"" : "")} class=""layui-form-item"">
    <label for=""{Id}"" class=""layui-form-label"" {(LabelWidth == null ? string.Empty : "style='width:" + LabelWidth + "px'")}>{lb}</label>
    <div class=""{ (string.IsNullOrEmpty(PaddingText) ? "layui-input-block" : "layui-input-inline")}"" {(LabelWidth == null ? "" : "style='margin-left:" + (LabelWidth + 30) + "px'")}>
";
            }
            else
            {
                preHtml += $@"
<div {(this is DisplayTagHelper ? "style=\"margin-bottom:0px;\"" : "")} class=""layui-form-item"">
    <div class=""{ (string.IsNullOrEmpty(PaddingText) ? "layui-input-block" : "layui-input-inline")}"" style=""margin-left:0px"">
";
            }
            if (string.IsNullOrEmpty(PaddingText))
            {
                postHtml += $@"
    </div>
</div>
";
            }
            else
            {
                postHtml += $@"
    </div>
<label class=""layui-form-label"" >{PaddingText}</label>
</div>
";

            }


            output.PreElement.SetHtmlContent(preHtml + output.PreElement.GetContent());
            output.PostElement.AppendHtml(postHtml);
            base.Process(context, output);
        }

    }
}

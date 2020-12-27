using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public enum ButtonSizeEnum { Big, Normal, Small, Mini }
    public enum ButtonThemeEnum { Primary, Normal, Warm, Danger, Disabled }
    public abstract class BaseButtonTag : BaseElementTag
    {
        /// <summary>
        /// 按钮尺寸,默认为Normal
        /// </summary>
        public ButtonSizeEnum? Size { get; set; }

        /// <summary>
        /// 按钮风格,默认为Normal
        /// </summary>
        public ButtonThemeEnum? Theme { get; set; }

        /// <summary>
        /// 按钮图标
        /// 图标字符串格式参考 http://www.layui.com/doc/element/icon.html
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 是否圆角
        /// </summary>
        public bool IsRound { get; set; }

        /// <summary>
        /// 按钮文字
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 点击事件调用的js方法，如doclick()
        /// </summary>
        public string Click { get; set; }

        public bool Disabled { get; set; }

        public string ConfirmTxt { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.NewGuid().ToNoSplitString();
            }
            if (output.TagName != "a")
            {
                output.TagName = "button";
                output.TagMode = TagMode.StartTagAndEndTag;
                var btnclass = "layui-btn";
                if (Size != null && Size != ButtonSizeEnum.Normal)
                {
                    switch (Size)
                    {
                        case ButtonSizeEnum.Big:
                            btnclass += " layui-btn-lg";
                            break;
                        case ButtonSizeEnum.Small:
                            btnclass += " layui-btn-sm";
                            break;
                        case ButtonSizeEnum.Mini:
                            btnclass += " layui-btn-xs";
                            break;
                        default:
                            break;
                    }
                }
                if(Disabled == true)
                {
                    Theme = ButtonThemeEnum.Disabled;
                    output.Attributes.SetAttribute(new TagHelperAttribute("disabled"));
                }
                if (Theme != null )
                {
                    btnclass += " layui-btn-" + Theme.Value.ToString().ToLower();
                }
                output.Attributes.SetAttribute("class", btnclass);
                if (string.IsNullOrEmpty(Icon) == false)
                {
                    output.Content.SetHtmlContent($@"<i class=""{Icon}""></i> {Text ?? ""}");
                }
                else
                {
                    output.Content.SetHtmlContent(Text ?? string.Empty);
                }
            }
            else
            {
                output.Content.SetHtmlContent(Text ?? string.Empty);
            }
            string submitButtonUrl = "";
            if (this is SubmitButtonTagHelper sbt)
            {
                if (string.IsNullOrEmpty(sbt.SubmitUrl) == false && context.Items.ContainsKey("formid") == true)
                {
                    submitButtonUrl = $"$('#{context.Items["formid"]}').attr('action','{sbt.SubmitUrl}');";
                }
            }
            string onclick = null;
            if (string.IsNullOrEmpty(Click) == false && Disabled == false)
            {
                if (!string.IsNullOrEmpty(ConfirmTxt))
                {
                    Click = $"layer.confirm('{ConfirmTxt}', {{icon: 3, title:'{Program._localizer["Info"]}'}}, function(index){{ {Click};layer.close(index); }})";
                }
                //if (this is SubmitButtonTagHelper)
                //{
                //    onclick = Click+";return true;";
                //}
                //else
                //{
                onclick = Click + ";return false;";
                //}
            }

            output.PostElement.AppendHtml($@"
<script>
  $('#{Id}').on('click',function(){{
    {submitButtonUrl}
    {onclick}
}});
</script>
");

            base.Process(context, output);
        }

    }
}

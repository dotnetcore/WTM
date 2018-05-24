using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public enum ButtonTargetEnum { Layer, self, newwindow }

    [HtmlTargetElement("wt:linkbutton", TagStructure = TagStructure.WithoutEndTag)]
    public class LinkButtonTagHelper : BaseButtonTag
    {
        /// <summary>
        /// 是否使用<a>的形式输出
        /// </summary>
        public bool IsLink { get; set; }

        /// <summary>
        /// 跳转模式
        /// </summary>
        public ButtonTargetEnum? Target { get; set; }

        /// <summary>
        /// 跳转地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 如果跳转模式是Layer，设置弹出窗口标题
        /// </summary>
        public string WindowTitle { get; set; }

        /// <summary>
        /// 如果跳转模式是Layer，设置弹出窗口宽度
        /// </summary>
        public int? WindowWidth { get; set; }

        /// <summary>
        /// 如果跳转模式是Layer，设置弹出窗口高度
        /// </summary>
        public int? WindowHeight { get; set; }

        /// <summary>
        /// 如果按钮在表单内，则使用表单数据Post到Url中
        /// </summary>
        public bool PostCurrentForm { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (IsLink == false)
            {
                output.Attributes.SetAttribute("type", "button");
            }
            else
            {
                output.TagName = "a";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.SetAttribute("href", "#");
            }
            if (Target == null || Target == ButtonTargetEnum.Layer)
            {
                string windowid = Guid.NewGuid().ToString();
                if (PostCurrentForm == true && context.Items.ContainsKey("formid"))
                {
                    Click = $"ff.OpenDialog('{Url}', '{windowid}', '{WindowTitle ?? ""}',{WindowWidth?.ToString() ?? "null"}, {WindowHeight?.ToString() ?? "null"}, ff.GetFormData('{context.Items["formid"]}'))";
                }
                else
                {
                    Click = $"ff.OpenDialog('{Url}', '{windowid}', '{WindowTitle ?? ""}',{WindowWidth?.ToString() ?? "null"}, {WindowHeight?.ToString() ?? "null"})";
                }
            }
            else if(Target == ButtonTargetEnum.self)
            {
                Click = $"ff.LoadPage('{Url}')";
            }
            else if(Target == ButtonTargetEnum.newwindow)
            {
                Click = $"ff.SetCookie('#{Url}','{WindowTitle??""}',true);window.open('/Home/PIndex#{Url}')";
            }
            base.Process(context, output);
        }

    }
}

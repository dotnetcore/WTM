using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI.Form
{
    [HtmlTargetElement("wt:ueditor", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class UEditorTagHelper : BaseFieldTag
    {
        //文本框为空显示的PlaceHolder
        public string EmptyText { get; set; }

        //定义高度
        public new int? Height { get; set; }

        //定义宽度
        public new int? Width { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string placeHolder = EmptyText ?? string.Empty;
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            string strWidth = Width == null ? "100%" : (Width + "px");
            string strHeight = Height == null ? "200px" : (Height + "px");
            output.Attributes.Add("style", $"width:{strWidth};height:{strHeight};");
            output.Attributes.Add("isrich", "1");

            output.PostElement.AppendHtml($@"
<script>
  layui.use(['ueditorconfig'], function () {{
    layui.ueditor.loadEditor('{Id}').ready(function(){{this.setContent('{(DefaultValue != null ? DefaultValue.ToString() : Field?.Model?.ToString())}')}});
  }});
</script>
");
            base.Process(context, output);
        }
    }
}

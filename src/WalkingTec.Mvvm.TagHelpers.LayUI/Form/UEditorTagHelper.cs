using System.Web;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;

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
        public string UploadGroupName { get; set; }
        public string UploadSubdir { get; set; }
        public string ConnectionString { get; set; }
        public string UploadMode { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string placeHolder = EmptyText ?? string.Empty;
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            string strWidth = Width == null ? "100%" : (Width + "px");
            string strHeight = Height == null ? "200px" : (Height + "px");
            output.Attributes.Add("style", $"width:{strWidth};height:{strHeight};");
            output.Attributes.Add("isrich", "1");
            var vm = context.Items["model"] as BaseVM;
            string url = "UploadForLayUIUEditor";
            if (string.IsNullOrEmpty(ConnectionString) == true)
            {
                if (vm != null)
                {
                    url = url.AppendQuery($"_DONOT_USE_CS={vm.CurrentCS}");
                }
            }
            else
            {
                url = url.AppendQuery($"_DONOT_USE_CS={ConnectionString}");
            }
            if (string.IsNullOrEmpty(UploadGroupName) == false)
            {
                url = url.AppendQuery($"groupName={UploadGroupName}");
            }
            if (string.IsNullOrEmpty(UploadSubdir) == false)
            {
                url = url.AppendQuery($"subdir={UploadSubdir}");
            }
            if (string.IsNullOrEmpty(UploadMode) == false)
            {
                url = url.AppendQuery($"sm={UploadMode}");
            }


            if (vm != null)
            {
                vm.ConfigInfo.UEditorOptions.FileActionName = url;
                vm.ConfigInfo.UEditorOptions.ImageActionName = url;
                vm.ConfigInfo.UEditorOptions.ScrawlActionName = url;
                vm.ConfigInfo.UEditorOptions.SnapscreenActionName = url;
                vm.ConfigInfo.UEditorOptions.VideoActionName = url;
            }
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

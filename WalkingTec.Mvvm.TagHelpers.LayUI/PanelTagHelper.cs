using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:panel", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class PanelTagHelper : BaseElementTag
    {
        /// <summary>
        /// 是否合上，默认是展开状态
        /// </summary>
        public bool Collapsed { get; set; }

        public string Title { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "layui-collapse");
            output.Attributes.SetAttribute("lay-accordion", "");
            var inside = await output.GetChildContentAsync();
            output.Content.SetHtmlContent($@"
<div class=""layui-colla-item"">
    <h2 class=""layui-colla-title"">{Title??""}</h2>
    <div class=""layui-colla-content {(Collapsed==true?"":"layui-show")}"">
        {inside.GetContent()}
    </div>
</div>
");
            output.PostElement.AppendHtml($@"
<script>
    var element = layui.element;
    element.init();
</script>
");
            base.Process(context, output);
        }
    }
}

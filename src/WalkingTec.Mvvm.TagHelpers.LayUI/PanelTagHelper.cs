using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public enum PanelType { Collapse, Card}

    [HtmlTargetElement("wt:panel", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class PanelTagHelper : BaseElementTag
    {
        /// <summary>
        /// 是否合上，默认是展开状态
        /// </summary>
        public bool Collapsed { get; set; }

        public string Title { get; set; }

        public PanelType PanelType { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            string tid = Guid.NewGuid().ToString().Replace("-", "");
            output.Attributes.Add("lay-filter", tid);
            if (PanelType == PanelType.Collapse)
            {
                output.Attributes.SetAttribute("class", "layui-collapse");
                output.Attributes.SetAttribute("lay-accordion", "");
                var inside = await output.GetChildContentAsync();
                output.Content.SetHtmlContent($@"
<div class=""layui-colla-item"">
    <h2 class=""layui-colla-title"">{Title ?? ""}</h2>
    <div class=""layui-colla-content {(Collapsed == true ? "" : "layui-show")}"">
        {inside.GetContent()}
    </div>
</div>
");
            }
            if(PanelType == PanelType.Card)
            {
                output.Attributes.SetAttribute("class", "layui-card");
                var inside = await output.GetChildContentAsync();
                output.Content.SetHtmlContent($@"
<div class=""layui-card-header"">{Title ?? ""}</div>
<div class=""layui-card-body"">
    {inside.GetContent()}
</div>
");

            }
            output.PostElement.AppendHtml($@"
<script>
    var element = layui.element;
    element.init();
element.on('collapse({tid})', function(data){{
setTimeout(function () {{ 
if (typeof(Event) === 'function') {{
  window.dispatchEvent(new Event('resize'));
}} else {{
  var evt = window.document.createEvent('UIEvents'); 
  evt.initUIEvent('resize', true, false, window, 0); 
  window.dispatchEvent(evt);
}}}}, 10);     
}});

</script>
");
            base.Process(context, output);
        }
    }
}

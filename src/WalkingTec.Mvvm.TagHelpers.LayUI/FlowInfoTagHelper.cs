using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.WorkFlow;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{


    [HtmlTargetElement("wt:flowinfo",TagStructure = TagStructure.WithoutEndTag)]
    public class FlowInfoTagHelper : BaseElementTag
    {
        public ModelExpression Vm { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            List<ApproveTimeLine> data = new List<ApproveTimeLine>(); 
            if (Vm?.Model is IBaseCRUDVM<TopBasePoco> vm)
            {
                data = vm.GetWorkflowTimeLineAsync().Result;
            }
            
            output.TagName = "ul";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", "layui-timeline");
            foreach (var item in data)
            {
                    output.Content.AppendHtml($@"
<li class=""layui-timeline-item"">
    <i class=""layui-icon layui-timeline-axis"">&#xe63f;</i>
    <div class=""layui-timeline-content layui-text"">
      <h3 class=""layui-timeline-title"">{item.Time}</h3>
      <p>
        {item.Message}
      </p>
      <p>{(string.IsNullOrEmpty(item.Remark) ? "": "备注：" + item.Remark)}</p>
    </div>
  </li>
");
                }
            
            base.Process(context, output);
        }
    }
}

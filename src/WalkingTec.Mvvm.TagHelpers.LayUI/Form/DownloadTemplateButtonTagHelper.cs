using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:downloadTemplateButton", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class DownloadTemplateButtonTagHelper : BaseButtonTag
    {
        protected const string REQUIRED_ATTR_NAME = "vm";
        public ModelExpression Vm { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var vmQualifiedName = Vm.Model.GetType().AssemblyQualifiedName;
            vmQualifiedName = vmQualifiedName.Substring(0, vmQualifiedName.LastIndexOf(", Version="));

            output.TagName = "a";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Attributes.SetAttribute("href", $"/_Framework/GetExcelTemplate?_DONOT_USE_VMNAME={vmQualifiedName}");
            Text = "下载模板";
            base.Process(context, output);
        }

    }
}

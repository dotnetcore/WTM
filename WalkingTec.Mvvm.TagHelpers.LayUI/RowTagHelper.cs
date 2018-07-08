using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public enum ItemsPerRowEnum { One=1, Two=2,Three=3,Four=4,Six=6,Twelve=12}
    public enum AlignEnum { Left, Right, Center}

    [HtmlTargetElement("wt:row", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class RowTagHelper : TagHelper
    {
        public ItemsPerRowEnum? ItemsPerRow { get; set; }

        public AlignEnum? Align { get; set; }

        public int Space { get; set; }
        public string Id { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            if (Space > 0)
            {
                output.Attributes.SetAttribute("class", $"layui-row layui-col-space{Space}");
            }
            else
            {
                output.Attributes.SetAttribute("class", $"layui-row ");
            }
            if (string.IsNullOrEmpty(Id) == false){
                output.Attributes.SetAttribute("id", Id);
            }
            switch (Align)
            {
                case AlignEnum.Left:
                    output.Attributes.SetAttribute("style", "width:100%;text-align:left");
                    break;
                case AlignEnum.Right:
                    output.Attributes.SetAttribute("style", "width:100%;text-align:right");
                    break;
                case AlignEnum.Center:
                    output.Attributes.SetAttribute("style", "width:100%;text-align:center");
                    break;
                default:
                    break;
            }
            if (context.Items.ContainsKey("ipr"))
            {
                context.Items["ipr"] = (int?)ItemsPerRow;
            }
            else
            {
                context.Items.Add("ipr", (int?)ItemsPerRow);
            }
        }

    }
}

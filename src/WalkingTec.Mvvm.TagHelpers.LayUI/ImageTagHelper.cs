using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:image", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class ImageTagHelper : BaseElementTag
    {
        protected const string REQUIRED_ATTR_NAME = "field";
        public ModelExpression Field { get; set; }

        public string Url { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var vm = context.Items["model"] as BaseVM;
            if (string.IsNullOrEmpty(Url) && Field.Model != null)
            {
                Url = $"/_Framework/GetFile/{Field.Model}";
                if (vm != null)
                {
                    Url += $"?_DONOT_USE_CS={vm.CurrentCS}";
                }
            }
            output.TagName = "img";
            output.TagMode = TagMode.SelfClosing;
            output.Attributes.Add("name", Field.Name + "img");
            output.Attributes.Add("id", Id + "img");
            if (!string.IsNullOrEmpty(Url))
                output.Attributes.Add("src", Url);
            base.Process(context, output);
        }

    }
}

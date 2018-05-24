using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public enum TabStyleEnum { Default, Simple }

    [HtmlTargetElement("wt:tab", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class TabTagHelper : BaseElementTag
    {
        public bool AllowClose { get; set; }
        public int SelectedIndex { get; set; }

        public TabStyleEnum TabStyle { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            this.Id = Guid.NewGuid().ToString().ToLower().Replace("-","");
            output.TagName = "div";
            if (TabStyle == TabStyleEnum.Default)
            {
                output.Attributes.SetAttribute("class", "layui-tab layui-tab-card");
            }
            else
            {
                output.Attributes.SetAttribute("class", "layui-tab layui-tab-brief");                
            }
            if(AllowClose == true)
            {
                output.Attributes.SetAttribute("lay-allowclose", "true");
            }
            //context.Items.Add("tabselectedindex", 0);
            output.PostElement.AppendHtml($@"
<script>
    $('#{Id} ul li').eq({SelectedIndex}).addClass('layui-this');
    $('#{Id} .layui-tab-item').eq({SelectedIndex}).addClass('layui-show');
</script>
");
            base.Process(context, output);
        }
    }

    [HtmlTargetElement("wt:tabheaders", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class TabHeadersTagHelper : BaseElementTag
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ul";
            output.Attributes.SetAttribute("class", "layui-tab-title");
            base.Process(context, output);
        }
    }

    [HtmlTargetElement("wt:tabheader", TagStructure = TagStructure.WithoutEndTag)]
    public class TabHeaderTagHelper : BaseElementTag
    {
        public string Title { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "li";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(Title);
            base.Process(context, output);
        }
    }

    [HtmlTargetElement("wt:tabcontents", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class TabContentsTagHelper : BaseElementTag
    {

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "layui-tab-content");
            base.Process(context, output);
        }
    }

    [HtmlTargetElement("wt:tabcontent", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class TabContentTagHelper : BaseElementTag
    {
        public string Url { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "layui-tab-item");
            base.Process(context, output);
        }
    }
}

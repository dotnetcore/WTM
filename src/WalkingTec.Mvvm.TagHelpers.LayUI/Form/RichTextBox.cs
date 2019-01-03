using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.TagHelpers.LayUI.Form
{
    [HtmlTargetElement("wt:richtextbox", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.NormalOrSelfClosing)]
    public class RichTextBoxTagHelper : BaseFieldTag
    {
        public string EmptyText { get; set; }
        public string UploadUrl { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string placeHolder = EmptyText ?? "";
            output.TagName = "textarea";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("placeholder", placeHolder);
            output.Attributes.Add("style", "display:none");
            output.Attributes.Add("isrich", "1");
            if (DefaultValue != null)
            {
                output.Content.SetContent(DefaultValue.ToString());
            }
            else
            {
                output.Content.SetContent(Field?.Model?.ToString());
            }
            string url = UploadUrl;
            if (string.IsNullOrEmpty(url))
            {
                url = "/_framework/UploadForLayUIRichTextBox";
            }
            if (context.Items.ContainsKey("model") == true)
            {
                var bvm = context.Items["model"] as BaseVM;
                if(bvm?.CurrentCS != null)
                {
                    url += $"?_DONOT_USE_CS={bvm.CurrentCS}";
                }
            }

            output.PostElement.AppendHtml($@"
<script>
layui.use('layedit', function(){{
  var layedit = layui.layedit;
layedit.set({{
  uploadImage: {{
    url: '{url}'
  }}
}});
  var index = layedit.build('{Id}'); 
  $('#{Id}').attr('layeditindex',index);
}});
</script>
");
            base.Process(context, output);
        }
    }

}

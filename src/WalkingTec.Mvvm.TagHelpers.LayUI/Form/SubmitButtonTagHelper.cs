using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:submitbutton", TagStructure = TagStructure.WithoutEndTag)]
    public class SubmitButtonTagHelper : BaseButtonTag
    {
        public string SubmitUrl { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string innerclick = Click;
            string formid = "";
            BaseVM vm = null;
            if (context.Items.ContainsKey("model") == true)
            {
                vm = context.Items["model"] as BaseVM;
            }
            output.Attributes.SetAttribute("type", "submit");
            output.Attributes.SetAttribute(new TagHelperAttribute("lay-submit"));
            if (context.Items.ContainsKey("formid"))
            {
                if(string.IsNullOrEmpty(this.Id) == true){
                    this.Id = Guid.NewGuid().ToString().Replace("-","");
                }
                output.Attributes.SetAttribute("lay-filter", context.Items["formid"] + "filter");
                formid = context.Items["formid"].ToString();
            }
            if (string.IsNullOrEmpty(Text))
            {
                Text = Program._localizer["Submit"];
            }
            if (string.IsNullOrEmpty(Click) == false || string.IsNullOrEmpty(ConfirmTxt) == false)
            {
                Click = $"f_{this.Id}Click();";
                output.Attributes.SetAttribute("lay-filter", "f_"+this.Id + "filter");
                output.PostElement.AppendHtml($@"
<script>
function f_{this.Id}Click(){{
    var check = eval(""{innerclick}"");
    if(check == undefined || check == false){{return false;}}
    try{{
        {formid}validate = false;
        $('#{formid}hidesubmit').trigger('click');
    }}
    catch(e){{ {formid}validate = true;}}
    if({formid}validate == true){{
    ff.PostForm('', '{formid}', '{vm?.ViewDivId}')
    }}
    return false;
}}
</script>
");
            }
            base.Process(context, output);
        }
    }
}

using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WalkingTec.Mvvm.Core.Extensions;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:colorpicker", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class ColorPickerTagHelper : BaseFieldTag
    {
        public string EmptyText { get; set; }

        /// <summary>
        /// 改变选择时触发的js函数，func(data)格式;
        /// <para>
        /// data为所选颜色;
        /// </para>
        /// </summary>
        public string ChangeFunc { get; set; }

        public bool EnableAlpha { get; set; }

        public string PredefinedColors { get; set; }
        public bool IsPassword { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.Add("id", $"cp_{Id}");

            string prec = "";
            var cs = PredefinedColors?.Split(",");
            if (cs != null)
            {
                foreach (var item in cs)
                {
                    if (item != "")
                    {
                        prec += $"'{item}',";
                    }
                }
            }
            if(prec.Length > 0)
            {
                prec = prec.Substring(0, prec.Length - 1);
            }
            string val = "";
            if (DefaultValue != null)
            {
                val = DefaultValue;
            }
            else
            {
                val = Field?.Model?.ToString();
            }
            var requiredtext = "";
            if (Field.Metadata.IsRequired)
            {
                requiredtext = $" lay-verify=\"required\" lay-reqText=\"{Program._localizer["{0}required", Field?.Metadata?.DisplayName ?? Field?.Metadata?.Name]}\"";
            }

            var content = $@"
<input type='hidden' id='{Id}' name='{Field.Name}' value='{val}' {requiredtext}/>
<script>
layui.use('colorpicker', function(){{
  var colorpicker = layui.colorpicker;
  colorpicker.render({{
    elem: '#cp_{Id}'
    ,color:'{val}'
    ,alpha : {EnableAlpha.ToString().ToLower()}
    ,format: '{(EnableAlpha==true? "rgb":"hex")}'
    ,predefine: {(PredefinedColors == null ? "false" : "true")}
    {(prec == "" ?"":$",colors: [{prec}]")}
    ,done: function(data){{
      $('#{Id}').val(data);
        {FormatFuncName(ChangeFunc)};
    }}
  }});
}});</script>
";
            output.PostElement.AppendHtml(content);

            base.Process(context, output);
        }
    }

}

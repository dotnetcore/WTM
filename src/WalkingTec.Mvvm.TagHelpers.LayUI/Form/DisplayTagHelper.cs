using Microsoft.AspNetCore.Razor.TagHelpers;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using System.Linq;
using System;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:display", TagStructure = TagStructure.WithoutEndTag)]
    public class DisplayTagHelper : BaseFieldTag
    {
        public string DisplayText { get; set; }

        public string Format { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {            
            bool isFile = false;
            if (Field?.Name?.ToLower().EndsWith("id") == true)
            {
                var file = Field.Metadata.ContainerType.GetProperties().Where(x => x.Name.ToLower() + "id" == Field.Metadata.PropertyName.ToLower()).FirstOrDefault();
                if(file != null)
                {
                    isFile = true;
                }
            }
            if (isFile == true)
            {
                if (Field.Model != null)
                {
                    output.TagName = "a";
                    output.TagMode = TagMode.StartTagAndEndTag;
                    output.Attributes.Add("class", "layui-btn layui-btn-primary layui-btn-xs");
                    output.Attributes.Add("style", "margin:9px 0;width:unset");
                    var vm = context.Items["model"] as BaseVM;
                    if (vm != null)
                    {
                        output.Attributes.Add("href", $"/_Framework/GetFile/{Field.Model}?_DONOT_USE_CS={vm.CurrentCS}");
                    }
                    else
                    {
                        output.Attributes.Add("href", $"/_Framework/GetFile/{Field.Model}");
                    }
                    output.Content.AppendHtml("下载");
                }
                else
                {
                    output.TagName = "label";
                    output.TagMode = TagMode.StartTagAndEndTag;
                    output.Attributes.Add("class", "layui-form-label");
                    output.Attributes.Add("style", "text-align:left;padding:9px 0;width:unset");
                    output.Content.AppendHtml("无");
                }
            }
            else
            {
                output.TagName = "label";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.Add("class", "layui-form-label");
                output.Attributes.Add("style", "text-align:left;padding:9px 0;width:unset");
                var val = string.Empty;
                if (Field?.Model != null)
                {
                    if (Field.Model.GetType().IsEnumOrNullableEnum())
                    {
                        val = PropertyHelper.GetEnumDisplayName(Field.Model.GetType(), Field.Model.ToString());
                    }
                    else if(Field.Model.GetType() == typeof(DateTime) || Field.Model.GetType() == typeof(DateTime?))
                    {
                        if (string.IsNullOrEmpty(Format))
                        {
                            val = Field.Model.ToString();
                        }
                        else
                        {
                            val = (Field.Model as DateTime?).Value.ToString(Format);
                        }
                    }
                    else if (Field.Model.GetType().IsBoolOrNullableBool())
                    {
                        if((bool?)Field.Model == true)
                        {
                            val = "是";
                        }
                        else
                        {
                            val = "否";
                        }
                    }
                    else
                    {
                        val = Field.Model.ToString();
                    }
                }
                else
                {
                    val = DisplayText;
                }
                output.Content.AppendHtml(val);
            }
            base.Process(context, output);
        }

    }
}

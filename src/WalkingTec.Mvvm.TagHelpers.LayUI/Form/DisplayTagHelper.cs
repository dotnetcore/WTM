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
                var file = Field.Metadata.ContainerType.GetSingleProperty(x => x.Name.ToLower() + "id" == Field.Metadata.PropertyName.ToLower()); 
                if (file != null && file.PropertyType == typeof(FileAttachment))
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
                    if (context.Items["model"] is BaseVM vm)
                    {
                        output.Attributes.Add("href", $"/_Framework/GetFile/{Field.Model}?_DONOT_USE_CS={vm.CurrentCS}");
                    }
                    else
                    {
                        output.Attributes.Add("href", $"/_Framework/GetFile/{Field.Model}");
                    }
                    output.Content.AppendHtml(THProgram._localizer["Sys.Download"]);
                }
                else
                {
                    output.TagName = "label";
                    output.TagMode = TagMode.StartTagAndEndTag;
                    output.Attributes.Add("class", "layui-form-label");
                    output.Attributes.Add("style", "text-align:left;padding:9px 0;width:unset;word-break:break-all;");
                    output.Content.AppendHtml(THProgram._localizer["Sys.None"]);
                }
            }
            else
            {
                output.TagName = "label";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.Add("class", "layui-form-label");
                output.Attributes.Add("style", "text-align:left;padding:9px 0;width:unset;word-break:break-all;");
                var val = string.Empty;
                if (Field?.Model != null)
                {
                    if (Field.Model.GetType().IsEnumOrNullableEnum())
                    {
                        val = PropertyHelper.GetEnumDisplayName(Field.Model.GetType(), Field.Model.ToString());
                    }
                    else if (Field.Model.GetType() == typeof(DateTime) || Field.Model.GetType() == typeof(DateTime?))
                    {
                        var datevalue = Field.Model as DateTime?;
                        if (datevalue != null)
                        {
                            if (string.IsNullOrEmpty(Format))
                            {
                                if (datevalue.Value.Hour == 0 && datevalue.Value.Minute == 0 && datevalue.Value.Second == 0)
                                {
                                    val = datevalue.Value.ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    val = datevalue.Value.ToString("yyyy-MM-dd HH:mm:ss");
                                }
                            }
                            else
                            {
                                val = (Field.Model as DateTime?).Value.ToString(Format);
                            }
                        }
                    }
                    else if (Field.Model.GetType().IsBoolOrNullableBool())
                    {
                        if ((bool?)Field.Model == true)
                        {
                            val = THProgram._localizer["Sys.Yes"];
                        }
                        else
                        {
                            val = THProgram._localizer["Sys.No"];
                        }
                    }
                    else if (Field.Model.GetType() == typeof(int) || Field.Model.GetType() == typeof(int?))
                    {
                        if (string.IsNullOrEmpty(Format))
                        {
                            val = Field.Model.ToString();
                        }
                        else
                        {
                            val = (Field.Model as int?).Value.ToString(Format);
                        }
                    }
                    else if (Field.Model.GetType() == typeof(long) || Field.Model.GetType() == typeof(long?))
                    {
                        if (string.IsNullOrEmpty(Format))
                        {
                            val = Field.Model.ToString();
                        }
                        else
                        {
                            val = (Field.Model as long?).Value.ToString(Format);
                        }
                    }
                    else if (Field.Model.GetType() == typeof(float) || Field.Model.GetType() == typeof(float?))
                    {
                        if (string.IsNullOrEmpty(Format))
                        {
                            val = Field.Model.ToString();
                        }
                        else
                        {
                            val = (Field.Model as float?).Value.ToString(Format);
                        }
                    }
                    else if (Field.Model.GetType() == typeof(double) || Field.Model.GetType() == typeof(double?))
                    {
                        if (string.IsNullOrEmpty(Format))
                        {
                            val = Field.Model.ToString();
                        }
                        else
                        {
                            val = (Field.Model as double?).Value.ToString(Format);
                        }
                    }
                    else if (Field.Model.GetType() == typeof(decimal) || Field.Model.GetType() == typeof(decimal?))
                    {
                        if (string.IsNullOrEmpty(Format))
                        {
                            val = Field.Model.ToString();
                        }
                        else
                        {
                            val = (Field.Model as decimal?).Value.ToString(Format);
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

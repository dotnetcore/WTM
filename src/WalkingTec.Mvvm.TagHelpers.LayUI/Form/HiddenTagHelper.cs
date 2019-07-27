using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WalkingTec.Mvvm.Core;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    [HtmlTargetElement("wt:hidden", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class HiddenTagHelper : BaseElementTag
    {
        protected const string REQUIRED_ATTR_NAME = "field";
        public ModelExpression Field { get; set; }
        public string Name { get; set; }
        private string _id;
        public new string Id
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    return Utils.GetIdByName(Field?.ModelExplorer.Container.ModelType.Name+"."+Field.Name) ?? string.Empty;
                }
                else
                {
                    return _id;
                }
            }
            set
            {
                _id = value;
            }
        }
        public string DefaultValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var value = string.Empty;
            var type = Field.Metadata.ModelType;
            if (Field.Model == null)
            {
                output.TagName = "input";
                output.TagMode = TagMode.StartTagOnly;
                output.Attributes.Add("type", "hidden");

                output.Attributes.Add("name", string.IsNullOrEmpty(Name) ? Field.Name : Name);

                if (DefaultValue != null)
                {
                    output.Attributes.Add("value", DefaultValue);
                }
                else
                {
                    output.Attributes.Add("value", value);
                }
                output.Attributes.Add("class", "layui-input");
            }
            else
            {
                var ss = typeof(IEnumerable<>);
                // 数组 or 泛型集合
                if (type.IsArray || (type.IsGenericType && typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
                {
                    var list = new List<object>();
                    foreach (var item in Field.Model as dynamic)
                    {
                        list.Add(item);
                    }
                    output.TagName = "div";
                    output.TagMode = TagMode.StartTagAndEndTag;
                    if (list.Count > 0)
                    {
                        var sb = new StringBuilder();
                        Type itemtype = list[0].GetType();

                        if (itemtype != typeof(object) && Type.GetTypeCode(itemtype) == TypeCode.Object && itemtype != typeof(Guid) && itemtype != typeof(Nullable<Guid>))
                        {
                            int count = 0;
                            foreach (var item in list)
                            {
                                var pros = itemtype.GetProperties();
                                foreach (var pro in pros)
                                {
                                    string name = Name;
                                    if (string.IsNullOrEmpty(name))
                                    {
                                        name = $"{Field.Name}[{count}].{pro.Name}";
                                    }
                                    sb.Append($@"<input type=""hidden"" name=""{name}"" value=""{pro.GetValue(item)}"" class=""layui-input""/>
");
                                }
                                count++;
                            }
                        }
                        else
                        {
                            foreach (var item in list)
                            {
                                sb.Append($@"<input type=""hidden"" name=""{(string.IsNullOrEmpty(Name) ? Field.Name : Name)}"" value=""{item}"" class=""layui-input""/>
");
                            }
                        }                        
                        output.PreContent.AppendHtml(sb.ToString());
                    }
                }
                else
                {
                    value = Field.Model.ToString();

                    output.TagName = "input";
                    output.TagMode = TagMode.StartTagOnly;
                    output.Attributes.Add("type", "hidden");

                    output.Attributes.Add("name", string.IsNullOrEmpty(Name) ? Field.Name : Name);

                    if (DefaultValue != null)
                    {
                        output.Attributes.Add("value", DefaultValue);
                    }
                    else
                    {
                        output.Attributes.Add("value", value);
                    }
                    output.Attributes.Add("class", "layui-input");
                    output.Attributes.SetAttribute("id", Id);
                }
            }

            base.Process(context, output);
        }
    }
}

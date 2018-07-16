using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using WalkingTec.Mvvm.Core;
using System.Linq;

namespace WalkingTec.Mvvm.TagHelpers.LayUI.Chart
{
    public enum ChartThemeEnum { light, dark}

    public enum ChartTypeEnum { Bar,Pie,Line}

    [HtmlTargetElement("wt:chart", TagStructure = TagStructure.WithoutEndTag)]
    public class ChartTagHelper : BaseElementTag
    {
        public ModelExpression Field { get; set; }

        public string Title { get; set; }

        public bool? ShowLegend { get; set; }

        public bool? ShowTooltip { get; set; }

        public ChartThemeEnum? Theme { get; set; }

        public ChartTypeEnum Type { get; set; }

        public bool IsHorizontal { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            Id = "chart"+Guid.NewGuid().ToString().Replace("-","");
            output.TagName = "div";
            output.Attributes.Add("ischart", "1");
            output.TagMode = TagMode.StartTagAndEndTag;
            var cd = Field?.Model as List<ChartData>;
            if(cd == null)
            {
                output.Content.SetContent("Field属性必须指定，且必须是List<ChartData>类型");
                return;
            }

            string legend = "";
            string tooltip = "";
            for(int i = 0; i < cd.Count; i++)
            {
                if (string.IsNullOrEmpty(cd[i].Series))
                {
                    cd[i].Series = "数据";
                }
            }

            string[] series = cd.Select(x => x.Series).Distinct().ToArray();

            if(ShowLegend == null && series.Length > 1)
            {
                ShowLegend = true;
            }
            if (ShowTooltip == null)
            {
                ShowTooltip = true;
            }
            if (ShowLegend == true)
            {
                legend = "legend: {},";
            }
            if (ShowTooltip == true)
            {
                tooltip = "tooltip: {},";
            }

            object[,] rtc = new object[cd.Count+1,series.Length+1];
            rtc[0, 0] = "'product'";
            for(int i = 0; i < series.Length; i++)
            {
                rtc[0, i + 1] = "'" + series[i] + "'";
            }
            for(int i = 0; i < cd.Count; i++)
            {
                rtc[i + 1, 0] = "'"+cd[i].Category+"'";
                for (int j = 0; j < series.Length; j++)
                {
                    if(cd[i].Series == series[j])
                    {
                        rtc[i + 1, j + 1] = cd[i].Value;
                        break;
                    }
                }

            }

            string data = "";
            data = "dataset: {source:[";
            for(int i = 0; i <= cd.Count; i++)
            {
                data += "[";
                for(int j = 0; j <= series.Length; j++)
                {
                    data += rtc[i, j];
                    if(j < series.Length)
                    {
                        data += ",";
                    }
                }
                data += "]";
                if (i < cd.Count )
                {
                    data += ",";
                }
            }
            data += "]},";

            string s = "series: [";
            for (int i = 0; i < series.Length; i++)
            {
                s += $"{{type:'{Type.ToString().ToLower()}'}}";
                if(i < series.Length - 1)
                {
                    s += ",";
                }
            }
            s += "]";

            string xAxis = "", yAxis = "";
            if (Type != ChartTypeEnum.Pie)
            {
                if (IsHorizontal == false)
                {
                    xAxis = "xAxis: {type: 'category'},";
                    yAxis = "yAxis: {},";
                }
                else
                {
                    xAxis = "xAxis: {},";
                    yAxis = "yAxis: {type: 'category'},";

                }
            }
            output.PostElement.AppendHtml($@"
<script>
        var {Id}Chart = echarts.init(document.getElementById('{Id}'), { (Theme==null ? "'default'":$"'{Theme.ToString()}'")});
        window.onresize = function(){{
            $(""div[ischart='1']"").each(function(index){{ eval($( this ).attr('id')+'Chart.resize();'); }});
        }};
        var {Id}option = {{
            {(string.IsNullOrEmpty(Title)?"":$"title:{{text: '{Title}'}},")}
            {tooltip}
            {legend}
            {xAxis}
            {yAxis}
            {data}
            {s}
        }};
        {Id}Chart.setOption({Id}option);
</script>
");
            base.Process(context, output);
        }
    }
}

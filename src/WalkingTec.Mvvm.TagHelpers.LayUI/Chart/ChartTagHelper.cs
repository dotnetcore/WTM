using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using WalkingTec.Mvvm.Core;
using System.Linq;

namespace WalkingTec.Mvvm.TagHelpers.LayUI.Chart
{
    public enum ChartThemeEnum { light, dark, vintage }

    public enum ChartTypeEnum { Bar, Pie, Line, PieHollow, Scatter }

    //[Obsolete("已废弃，预计v3.0版本及v2.10版本开始将删除 wt:chart")]
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
        //折线图弧度
        public bool OpenSmooth { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            Id = "chart" + Guid.NewGuid().ToString("N");
            output.TagName = "div";
            output.Attributes.Add("ischart", "1");
            output.TagMode = TagMode.StartTagAndEndTag;
            var cd = Field?.Model as List<ChartData>;
            if (cd == null)
            {
                output.Content.SetContent("Field must be set, and has to be of type List<ChartData>");
                return;
            }

            string legend = "";
            string tooltip = "";
            var i = 0;
            for (i = 0; i < cd.Count; i++)
            {
                if (string.IsNullOrEmpty(cd[i].Series))
                {
                    cd[i].Series = "数据";
                }
            }


            string[] series = cd.Select(x => x.Series).Distinct().ToArray();

            if (ShowLegend == null && series.Length > 1)
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

            var yCount = cd.GroupBy(x => x.Category).ToList();
            object[,] rtc = new object[yCount.Count + 1, series.Length + 1];
            rtc[0, 0] = "'product'";

            for (i = 0; i < series.Length; i++)
            {
                rtc[0, i + 1] = "'" + series[i] + "'";
            }
            //for (int i = 0; i < yCount.Count; i++)
            //{
            //    rtc[i + 1, 0] = "'" + yCount. + "'";
            //    for (int j = 0; j < series.Length; j++)
            //    {
            //        if (cd[i].Series == series[j])
            //        {
            //            rtc[i + 1, j + 1] = cd[i].Value;
            //            break;
            //        }
            //    }

            //}
            i = 0;
            foreach (var item in yCount)
            {
                rtc[i + 1, 0] = "'" + item.Key + "'";
                for (int j = 0; j < series.Length; j++)
                {
                    var ser = item.Where(x => x.Series == series[j])?.FirstOrDefault();
                    if (ser != null)
                    {
                        rtc[i + 1, j + 1] = ser.Value;
                    }
                }

                i++;
            }


            string data = "";
            data = "dataset: {source:[";
            for (i = 0; i <= yCount.Count; i++)
            {
                data += "[";
                for (int j = 0; j <= series.Length; j++)
                {
                    data += rtc[i, j];
                    if (j < series.Length)
                    {
                        data += ",";
                    }
                }
                data += "]";
                if (i < yCount.Count)
                {
                    data += ",";
                }
            }
            data += "]},";

            string s = "series: [";
            for (i = 0; i < series.Length; i++)
            {
                if (Type == ChartTypeEnum.PieHollow)
                    s += $"{{type:'pie',radius: ['40%', '70%']";
                else
                    s += $"{{type:'{Type.ToString().ToLower()}'";
                if (OpenSmooth && Type == ChartTypeEnum.Line)
                {
                    s += ",smooth: true";
                }
                s += "}";
                if (i < series.Length - 1)
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
var {Id}Chart;
layui.use(['echarts'],function(){{
  var echarts = layui.echarts;
  {Id}Chart = echarts.init(document.getElementById('{Id}'), {(Theme == null ? "'default'" : $"'{Theme.ToString()}'")});
  window.onresize = function(){{
    $(""div[ischart='1']"").each(function(index){{ eval($( this ).attr('id')+'Chart.resize();'); }});
  }};
  var {Id}option = {{
    {(string.IsNullOrEmpty(Title) ? "" : $"title:{{text: '{Title}'}},")}
    {tooltip}
    {legend}
    {xAxis}
    {yAxis}
    {data}
    {s}
  }};
  {Id}Chart.setOption({Id}option);
}});
</script>
");
            base.Process(context, output);
        }
    }
}

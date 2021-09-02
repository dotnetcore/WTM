using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using WalkingTec.Mvvm.Core;
using System.Linq;

namespace WalkingTec.Mvvm.TagHelpers.LayUI.Chart
{
    public enum ChartThemeEnum { light, dark, vintage, chalk, essos, macarons, roma, walden, westeros, wonderland }

    public enum ChartTypeEnum { Bar, Pie, Line, PieHollow, Scatter }

    //[Obsolete("已废弃，预计v3.0版本及v2.10版本开始将删除 wt:chart")]
    [HtmlTargetElement("wt:chart", TagStructure = TagStructure.WithoutEndTag)]
    public class ChartTagHelper : BaseElementTag
    {
        //public ModelExpression Field { get; set; }

        public string Title { get; set; }

        public bool? ShowLegend { get; set; }

        public bool? ShowTooltip { get; set; }

        public ChartThemeEnum? Theme { get; set; }

        public ChartTypeEnum Type { get; set; }

        public bool IsHorizontal { get; set; }
        //折线图弧度
        public bool OpenSmooth { get; set; }

        public string TriggerUrl { get; set; }
        public int Radius { get; set; } = 100;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            Id = "chart" + Guid.NewGuid().ToString("N");
            output.TagName = "div";
            output.Attributes.Add("ischart", "1");
            output.TagMode = TagMode.StartTagAndEndTag;
            //var cd = Field?.Model as List<ChartData>;
            //if (cd == null)
            //{
            //    output.Content.SetContent("Field must be set, and has to be of type List<ChartData>");
            //    return;
            //}

            string legend = "";
            string tooltip = "";




            if (ShowLegend == null)
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

            var typeSeries = string.Empty;
            if (Type == ChartTypeEnum.PieHollow)
                typeSeries = $"\"type\":\"pie\",\"radius\": [\"40%\", \"70%\"]";
            else
                typeSeries = $"\"type\":\"{Type.ToString().ToLower()}\"";
            if (OpenSmooth && Type == ChartTypeEnum.Line)
                typeSeries += ",\"smooth\": true";


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
                if (Type == ChartTypeEnum.Scatter)
                {
                    xAxis = "xAxis: { type: 'value',splitLine: { lineStyle: { type: 'dashed'} } },";
                    yAxis = "yAxis:{splitLine:{lineStyle:{type: 'dashed'} },scale: true},";
                }
            }
            if (Type == ChartTypeEnum.Scatter && ShowLegend == true)
            {
                legend = "legend:JSON.parse(data.legend),";
            }
            output.PostElement.AppendHtml($@"
<script>
var {Id}Chart;
var themeTemp ={(Theme == null ? "'default'" : $"'{Theme.ToString()}'")};
{Id}Chart = echarts.init(document.getElementById('{Id}'),themeTemp);
  window.onresize = function(){{
    $(""div[ischart='1']"").each(function(index){{ eval($( this ).attr('id')+'Chart.resize();'); }});
  }};
  var {Id}option; 
$.get('{TriggerUrl}').done(function (data) {{
    if(data.series!=undefined){{
        data.series=data.series.replaceAll('""type"":""charttype""','{typeSeries}');
    }}
  {Id}Chart.setOption({{
    {(string.IsNullOrEmpty(Title) ? "" : $"title:{{text: '{Title}'}},")}
    {tooltip}
    {legend}
    {xAxis}
    {yAxis}
    dataset:JSON.parse(data.dataset),
    series:JSONfns.parse(data.series) 
  }});
  
}});

</script>
");
            base.Process(context, output);
        }
    }
}

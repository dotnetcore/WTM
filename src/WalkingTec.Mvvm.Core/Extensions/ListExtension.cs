using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WalkingTec.Mvvm.Core.Extensions
{
    public static class ListExtension
    {
        /// <summary>
        /// 将数据的List转化为下拉菜单数据List
        /// </summary>
        /// <typeparam name="T">源数据类</typeparam>
        /// <param name="self">源数据List</param>
        /// <param name="textField">指向text字段的表达式</param>
        /// <param name="valueField">指向value字段的表达式</param>
        /// <param name="selectedCondition">默认被选中的条件</param>
        /// <returns>下拉菜单数据List</returns>
        public static List<ComboSelectListItem> ToListItems<T>(this List<T> self
            , Expression<Func<T, object>> textField
            , Expression<Func<T, object>> valueField
            , Expression<Func<T, bool>> selectedCondition = null)
        {
            var rv = new List<ComboSelectListItem>();
            if (self != null)
            {
                //循环列表中的数据
                foreach (var item in self)
                {
                    //获取textField的值作为text
                    string text = textField.Compile().Invoke(item).ToString();
                    //获取valueField的值作为value
                    string value = valueField.Compile().Invoke(item).ToString();
                    //添加到下拉菜单List中
                    ComboSelectListItem li = new ComboSelectListItem();
                    li.Text = text;
                    li.Value = value;
                    //如果有默认选择的条件，则将当前数据带入到判断表达式中，如果返回true，则将下拉数据的selected属性设为true
                    if (selectedCondition != null)
                    {
                        if (selectedCondition.Compile().Invoke(item))
                        {
                            li.Selected = true;
                        }
                    }
                    rv.Add(li);
                }
            }
            return rv;
        }

        public static object ToChartData<T>(this List<T> self, int radius = 100, string seriesname = "Info")
        {
            //var data = string.Empty;
            if (self != null && self.Count > 0)
            {
                var cd = self as List<ChartData>;
                var i = 0;
                for (i = 0; i < cd.Count; i++)
                {
                    if (string.IsNullOrEmpty(cd[i].Series))
                    {
                        cd[i].Series = "Data";
                    }
                }
                string[] series = cd.Select(x => x.Series).Distinct().ToArray();


                var yCount = cd.GroupBy(x => x.Category).ToList();
                var isScatter = cd.Any(x => x.ValueX > 0);
                var dataset = "{\"source\":[";
                if (isScatter)
                {
                    dataset = "[{\"source\":[";
                    i = 0;
                    foreach (var item in cd)
                    {

                        dataset += $"[{item.ValueX},{item.Value},{item.Addition},\"{item.Category}\",\"{item.Series}\"]";

                        if (i < cd.Count - 1)
                        {
                            dataset += ",";
                        }
                        i++;
                    }
                    dataset += "]},";
                    for (i = 0; i < series.Length; i++)
                    {
                        dataset += $"{{\"transform\": {{\"type\": \"filter\",\"config\": {{\"dimension\": 4,\"value\": \"{series[i]}\"}}}}}}";
                        if (i < series.Length - 1)
                        {
                            dataset += ",";
                        }
                    }
                    dataset += "]";
                }
                else
                {
                    object[,] rtc = new object[yCount.Count + 1, series.Length + 1];
                    rtc[0, 0] = $"{seriesname}";

                    for (i = 0; i < series.Length; i++)
                    {
                        rtc[0, i + 1] = series[i];
                    }
                    i = 0;
                    foreach (var item in yCount)
                    {
                        rtc[i + 1, 0] = item.Key;
                        for (int j = 0; j < series.Length; j++)
                        {
                            var ser = item.Where(x => x.Series == series[j])?.FirstOrDefault();
                            if (ser != null)
                            {
                                rtc[i + 1, j + 1] = ser.Value;
                            }
                            else
                            {
                                rtc[i + 1, j + 1] = 0;
                            }
                        }

                        i++;
                    }
                    for (i = 0; i <= yCount.Count; i++)
                    {
                        dataset += "[";
                        for (int j = 0; j <= series.Length; j++)
                        {
                            dataset += $"\"{rtc[i, j]}\"";
                            if (j < series.Length)
                            {
                                dataset += ",";
                            }
                        }
                        dataset += "]";
                        if (i < yCount.Count)
                        {
                            dataset += ",";
                        }
                    }
                    dataset += "]}";
                }
                var seriesStr = "[";
                var legend = "{\"data\":[";
                var max = cd.Max(x => x.Addition);
                for (i = 0; i < series.Length; i++)
                {
                    seriesStr += $"{{\"type\":\"charttype\"";
                    if (isScatter)
                    {
                        seriesStr += $",\"encode\":{{\"x\":0,\"y\":1,\"tooltip\":[2,3]}},\"name\": \"{series[i]}\",\"datasetIndex\": {i + 1}";
                        seriesStr += $",\"symbolSize\": \"function(data) {{return data[2] / ({max} / {radius})}}\"";
                    }
                    seriesStr += "}";
                    legend += $"\"{series[i]}\"";
                    if (i < series.Length - 1)
                    {
                        seriesStr += ",";
                        legend += ",";
                    }
                }
                legend += "]}";
                seriesStr += "]";
                return new { dataset = dataset, series = seriesStr, legend = legend };
            }
            else
            {
                var dataset = "{\"source\":[]}";
                var seriesStr = "[]";
                var legend = "{\"data\":[]}";
                return new { dataset = dataset, series = seriesStr, legend = legend };
            }
        }

    }
}

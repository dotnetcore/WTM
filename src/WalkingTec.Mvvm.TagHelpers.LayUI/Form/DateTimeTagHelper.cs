using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;

namespace WalkingTec.Mvvm.TagHelpers.LayUI
{
    public enum DateTimeLangEnum
    {
        /// <summary>
        /// 中文版，默认
        /// </summary>
        CN = 0,
        /// <summary>
        /// 即英文版
        /// </summary>
        EN
    }
    public enum DateTimeTypeEnum
    {
        /// <summary>
        /// 日期选择器
        /// 可选择：年、月、日
        /// </summary>
        Date,
        /// <summary>
        /// 日期时间选择器
        /// 可选择：年、月、日、时、分、秒
        /// </summary>
        Datetime,
        /// <summary>
        /// 年选择器
        /// 只提供年列表选择
        /// </summary>
        Year,
        /// <summary>
        /// 年月选择器
        /// 只提供年、月选择
        /// </summary>
        Month,
        /// <summary>
        /// 时间选择器
        /// 只提供时、分、秒选择
        /// </summary>
        Time
    }

    [HtmlTargetElement("wt:datetime", Attributes = REQUIRED_ATTR_NAME, TagStructure = TagStructure.WithoutEndTag)]
    public class DateTimeTagHelper : BaseFieldTag
    {
        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 控件选择类型
        /// 默认值：date
        /// </summary>
        public DateTimeTypeEnum Type { get; set; }

        /// <summary>
        /// 开启时间范围选择，默认 分隔符为 ~
        /// 默认值：false
        /// </summary>
        public bool? Range { get; set; }

        /// <summary>
        /// 时间范围分隔符 默认 ~
        /// </summary>
        public string RangeSplit { get; set; }

        /// <summary>
        /// Max
        /// 1.  如果值为字符类型，则：年月日必须用 -（中划线）分割、时分秒必须用 :（半角冒号）号分割。这里并非遵循 format 设定的格式
        /// 2.  如果值为整数类型，且数字＜86400000，则数字代表天数，如：min: -7，即代表最小日期在7天前，正数代表若干天后
        /// 3.  如果值为整数类型，且数字 ≥ 86400000，则数字代表时间戳，如：max: 4073558400000，即代表最大日期在：公元3000年1月1日
        /// </summary>
        public string Max { get; set; }

        /// <summary>
        /// Min
        /// 1.  如果值为字符类型，则：年月日必须用 -（中划线）分割、时分秒必须用 :（半角冒号）号分割。这里并非遵循 format 设定的格式
        /// 2.  如果值为整数类型，且数字 ＜ 86400000，则数字代表天数，如：min: -7，即代表最小日期在7天前，正数代表若干天后
        /// 3.  如果值为整数类型，且数字 ≥ 86400000，则数字代表时间戳，如：max: 4073558400000，即代表最大日期在：公元3000年1月1日
        /// </summary>
        public string Min { get; set; }

        /// <summary>
        /// 自定义格式 默认值：yyyy-MM-dd
        /// yyyy    年份，至少四位数。如果不足四位，则前面补零
        /// y       年份，不限制位数，即不管年份多少位，前面均不补零
        /// MM      月份，至少两位数。如果不足两位，则前面补零。
        /// M       月份，允许一位数。
        /// dd      日期，至少两位数。如果不足两位，则前面补零。
        /// d       日期，允许一位数。
        /// HH      小时，至少两位数。如果不足两位，则前面补零。
        /// H       小时，允许一位数。
        /// mm      分钟，至少两位数。如果不足两位，则前面补零。
        /// m       分钟，允许一位数。
        /// ss      秒数，至少两位数。如果不足两位，则前面补零。
        /// s       秒数，允许一位数。
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 层叠顺序
        /// 类型：Number，默认值：66666666
        /// 一般用于解决与其它元素的互相被遮掩的问题。如果 position 参数设为 static 时，该参数无效。
        /// </summary>
        public int? ZIndex { get; set; }

        /// <summary>
        /// 是否显示底部栏
        /// </summary>
        public bool? ShowBottom { get; set; }

        /// <summary>
        /// 语言 默认CN
        /// </summary>
        public DateTimeLangEnum? Lang { get; set; }

        /// <summary>
        /// 是否显示公历节日
        /// 内置了一些我国通用的公历重要节日，通过设置 true 来开启。国际版不会显示。
        /// </summary>
        public bool? Calendar { get; set; }

        /// <summary>
        /// 标注重要日子
        /// 标注          格式                    说明
        /// 每年的日期   {'0-9-18': '国耻'}        0 即代表每一年
        /// 每月的日期   {'0-0-15': '中旬'}        0-0 即代表每年每月（layui 2.1.1/layDate 5.0.4 新增）
        /// 特定的日期   {'2017-8-21': '发布'}     -
        /// </summary>
        public Dictionary<string, string> Mark { get; set; }

        /// <summary>
        /// 控件初始打开的回调，控件在打开时触发。
        /// 回调返回2个参数：初始的日期时间对象、当前实例对象
        /// </summary>
        public string ReadyFunc { get; set; }

        /// <summary>
        /// 日期时间被切换后的回调
        /// 年月日时间被切换时都会触发。
        /// 回调返回4个参数，分别代表：生成的值、日期时间对象、结束的日期时间对象、当前实例对象
        /// </summary>
        public string ChangeFunc { get; set; }

        /// <summary>
        /// 控件选择完毕后的回调
        /// 点击日期、清空、现在、确定均会触发。
        /// 回调返回4个参数，分别代表：生成的值、日期时间对象、结束的日期时间对象、当前实例对象
        /// </summary>
        public string DoneFunc { get; set; }

        public static Dictionary<DateTimeTypeEnum, string> DateTimeFormatDic = new Dictionary<DateTimeTypeEnum, string>()
        {
            { DateTimeTypeEnum.Date,"yyyy-MM-dd"},
            { DateTimeTypeEnum.Datetime,"yyyy-MM-dd HH:mm:ss"},
            { DateTimeTypeEnum.Year,"yyyy"},
            { DateTimeTypeEnum.Month,"MM"},
            { DateTimeTypeEnum.Time,"HH:mm:ss"},
        };
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "input";
            output.TagMode = TagMode.StartTagOnly;
            output.Attributes.Add("type", "text");
            output.Attributes.Add("name", Field.Name);
            if (Field.ModelExplorer.ModelType == typeof(string))
            {
                Value = Field.Model?.ToString() ?? Value;
            }
            else
            {
                Value = (Field.Model as DateTime?)?.ToString(DateTimeFormatDic[Type]) ?? Value;
            }
            output.Attributes.Add("value", Value);
            output.Attributes.Add("class", "layui-input");

            if (Range.HasValue && Range.Value && string.IsNullOrEmpty(RangeSplit))
            {
                RangeSplit = "~";
            }

            if (!string.IsNullOrEmpty(Min))
            {
                if (int.TryParse(Min, out int minRes))
                {
                    Min = minRes.ToString();
                }
                else
                {
                    Min = $"'{Min}'";
                }
            }
            if (!string.IsNullOrEmpty(Max))
            {
                if (int.TryParse(Max, out int maxRes))
                {
                    Max = maxRes.ToString();
                }
                else
                {
                    Max = $"'{Max}'";
                }
            }
            var content = $@"
<script>
    var laydate = layui.laydate;
    var ins1 = laydate.render({{
        elem: '#{Id}',
        type: '{Type.ToString().ToLower()}'
        {(string.IsNullOrEmpty(RangeSplit) ? string.Empty : $",range:'{RangeSplit}'")}
        {(string.IsNullOrEmpty(Format) ? string.Empty : $",format: '{Format}'")}
        {(string.IsNullOrEmpty(Min) ? string.Empty : $",min: {Min}")}
        {(string.IsNullOrEmpty(Max) ? string.Empty : $",max: {Max}")}
        {(!ZIndex.HasValue ? string.Empty : $",zIndex: {ZIndex.Value}")}
        {(!ShowBottom.HasValue ? string.Empty : $",showBottom: {ShowBottom.Value.ToString().ToLower()}")}
        {(!Calendar.HasValue ? string.Empty : $",calendar: {Calendar.Value.ToString().ToLower()}")}
        {(!Lang.HasValue ? string.Empty : $",lang: '{Lang.Value.ToString().ToLower()}'")}
        {(Mark == null || Mark.Count == 0 ? string.Empty : $",mark: {JsonConvert.SerializeObject(Mark)}")}
        {(string.IsNullOrEmpty(ReadyFunc) ? string.Empty : $",ready: function(value){{{ReadyFunc}(value,ins1)}}")}
        {(string.IsNullOrEmpty(ChangeFunc) ? string.Empty : $",change: function(value,date,endDate){{{ChangeFunc}(value,date,endDate,ins1)}}")}
        {(string.IsNullOrEmpty(DoneFunc) ? string.Empty : $",done: function(value,date,endDate){{{DoneFunc}(value,date,endDate,ins1)}}")}
        //,theme: 'molv',btns: ['clear','now','confirm']
    }});
</script>
";
            output.PostElement.AppendHtml(content);
            base.Process(context, output);
        }
    }
}

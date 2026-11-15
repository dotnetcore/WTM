using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WalkingTec.Mvvm.Core
{
    /// <summary>
    /// 日期范围类，表示由开始时间和结束时间组成的时间区间，
    /// 支持日期、日期时间、年、月、时间等多种类型，并可在DateTime、DateTimeOffset、TimeSpan及字符串之间相互转换
    /// </summary>
    public sealed class DateRange
    {
        //UTC模式下的默认纪元时间(1970-01-01 UTC)
        private static readonly DateTime UtCDefaultEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        //默认纪元时间(1970-01-01 本地时间)
        private static readonly DateTime DefaultEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        //默认的日期类型为DateTime
        private static readonly DateTimeTypeEnum DefaultType = DateTimeTypeEnum.DateTime;


        /// <summary>
        /// 私有无参构造函数，不允许外部以无参方式创建日期范围
        /// </summary>
        private DateRange() { }


        /// <summary>
        /// 以开始时间和结束时间构造日期范围，日期类型默认为DateTime，纪元默认为本地时间
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        public DateRange(DateTime startTime, DateTime endTime) : this(startTime, endTime, DefaultType, DefaultEpoch) { }

        /// <summary>
        /// 以开始时间、结束时间、日期类型和纪元时间构造日期范围
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="type">日期类型</param>
        /// <param name="epoch">时间戳纪元基准时间</param>
        public DateRange(DateTime startTime, DateTime endTime, DateTimeTypeEnum type, DateTime epoch)
        {
            Type = type;
            Epoch = epoch;
            SetStartTime(startTime);
            SetEndTime(endTime);
        }


        /// <summary>
        /// 以相对纪元的时间间隔构造日期范围，日期类型默认为DateTime，纪元默认为本地时间
        /// </summary>
        /// <param name="startSpan">开始时间相对纪元的偏移量</param>
        /// <param name="endSpan">结束时间相对纪元的偏移量</param>
        public DateRange(TimeSpan startSpan, TimeSpan endSpan) : this(startSpan, endSpan, DefaultType, DefaultEpoch) { }

        /// <summary>
        /// 以相对纪元的时间间隔、日期类型和纪元时间构造日期范围
        /// </summary>
        /// <param name="startSpan">开始时间相对纪元的偏移量</param>
        /// <param name="endSpan">结束时间相对纪元的偏移量</param>
        /// <param name="type">日期类型</param>
        /// <param name="epoch">时间戳纪元基准时间</param>
        public DateRange(TimeSpan startSpan, TimeSpan endSpan, DateTimeTypeEnum type, DateTime epoch)
        {
            Type = type;
            Epoch = epoch;
            SetStartTime(epoch.Add(startSpan));
            SetEndTime(epoch.Add(endSpan));
        }

        /// <summary>
        /// 以DateTimeOffset形式的开始和结束时间构造日期范围，日期类型默认为DateTime，纪元默认为本地时间
        /// </summary>
        /// <param name="startOffset">开始时间的DateTimeOffset表示</param>
        /// <param name="endOffset">结束时间的DateTimeOffset表示</param>
        public DateRange(DateTimeOffset startOffset, DateTimeOffset endOffset) : this(startOffset, endOffset, DefaultType, DefaultEpoch) { }

        /// <summary>
        /// 以DateTimeOffset形式的开始时间、结束时间、日期类型和纪元时间构造日期范围
        /// </summary>
        /// <param name="startOffset">开始时间的DateTimeOffset表示</param>
        /// <param name="endOffset">结束时间的DateTimeOffset表示</param>
        /// <param name="type">日期类型</param>
        /// <param name="epoch">时间戳纪元基准时间</param>
        public DateRange(DateTimeOffset startOffset, DateTimeOffset endOffset, DateTimeTypeEnum type, DateTime epoch)
        {
            Type = type;
            Epoch = epoch;
            SetStartTime(startOffset.DateTime);
            SetEndTime(endOffset.DateTime);
        }

        /// <summary>
        /// 时间戳纪元基准时间
        /// </summary>
        public readonly DateTime Epoch = DefaultEpoch;

        /// <summary>
        /// 日期范围类型
        /// </summary>
        public readonly DateTimeTypeEnum Type = DefaultType;


        /// <summary>
        /// 日期范围的字符串表示，格式为 开始时间 ~ 结束时间
        /// </summary>
        public string Value => ToString();


        //开始时间
        private DateTime? _startTime;

        /// <summary>
        /// 获取开始时间
        /// </summary>
        /// <returns>开始时间，未设置时返回null</returns>
        public DateTime? GetStartTime()
        {
            return _startTime;
        }

        /// <summary>
        /// 设置开始时间；若给定值晚于当前结束时间，则不做任何修改
        /// </summary>
        /// <param name="value">要设置的开始时间</param>
        public void SetStartTime(DateTime? value)
        {
            if (value == null) return;

            value = SwitchTime(value.Value);

            if (_endTime.HasValue && _endTime.Value < value) return;
            _startTime = value;
        }

        //结束时间
        private DateTime? _endTime;

        /// <summary>
        /// 获取结束时间：Date类型自动延后一天；DateTime类型下若结束时间为零点或与开始时间相同，同样延后一天，以保证查询能覆盖整个结束日
        /// </summary>
        /// <returns>结束时间，未设置时返回null</returns>
        public DateTime? GetEndTime()
        {
            switch (Type)
            {
                case DateTimeTypeEnum.Date:
                    return _endTime?.AddDays(1);
                case DateTimeTypeEnum.DateTime when _startTime.HasValue && _endTime.HasValue && _startTime.Value == _endTime.Value:
                case DateTimeTypeEnum.DateTime when _endTime.HasValue && _endTime.Value.Hour == 0 && _endTime.Value.Minute == 0 && _endTime.Value.Second == 0 && _endTime.Value.Millisecond == 0:
                    return _endTime?.AddDays(1);
                default:
                    return _endTime;
            }
        }

        /// <summary>
        /// 设置结束时间；若给定值早于当前开始时间，则不做任何修改
        /// </summary>
        /// <param name="value">要设置的结束时间</param>
        public void SetEndTime(DateTime? value)
        {
            if (value == null) return;

            value = SwitchTime(value.Value);

            if (_startTime.HasValue && _startTime.Value > value) return;
            _endTime = value;
        }


        /// <summary>
        /// 以DateTimeOffset形式获取开始时间
        /// </summary>
        /// <returns>开始时间的DateTimeOffset表示，未设置时返回null</returns>
        public DateTimeOffset? GetStartOffset()
        {
            if (_startTime == null)
                return null;
            return _startTime <= DateTimeOffset.MinValue.LocalDateTime ? DateTimeOffset.MinValue : new DateTimeOffset(GetStartTime().Value);
        }

        /// <summary>
        /// 以DateTimeOffset形式设置开始时间
        /// </summary>
        /// <param name="value">要设置的开始时间</param>
        public void SetStartOffset(DateTimeOffset? value)
        {
            if (value == null) return;
            SetStartTime(value.Value.LocalDateTime);
        }

        /// <summary>
        /// 以DateTimeOffset形式获取结束时间
        /// </summary>
        /// <returns>结束时间的DateTimeOffset表示，未设置时返回null</returns>
        public DateTimeOffset? GetEndOffset()
        {
            if (_endTime == null)
                return null;
            return _endTime <= DateTimeOffset.MinValue.LocalDateTime ? DateTimeOffset.MinValue : new DateTimeOffset(GetEndTime().Value);
        }

        /// <summary>
        /// 以DateTimeOffset形式设置结束时间
        /// </summary>
        /// <param name="value">要设置的结束时间</param>
        public void SetEndOffset(DateTimeOffset? value)
        {
            if (value == null) return;
            SetEndTime(value.Value.LocalDateTime);
        }


        /// <summary>
        /// 获取开始时间相对纪元的时间间隔
        /// </summary>
        /// <returns>开始时间相对纪元的偏移量，结束时间未设置时返回null</returns>
        public TimeSpan? GetStartSpan()
        {
            if (_endTime == null)
                return null;
            return GetStartTime() - Epoch.ToLocalTime();
        }

        /// <summary>
        /// 以相对纪元的时间间隔设置开始时间
        /// </summary>
        /// <param name="value">开始时间相对纪元的偏移量</param>
        public void SetStartSpan(TimeSpan? value)
        {
            if (value == null) return;
            SetStartTime(Epoch.Add(value.Value));
        }

        /// <summary>
        /// 获取结束时间相对纪元的时间间隔
        /// </summary>
        /// <returns>结束时间相对纪元的偏移量，结束时间未设置时返回null</returns>
        public TimeSpan? GetEndSpan()
        {
            if (_endTime == null)
                return null;
            return GetEndTime() - Epoch.ToLocalTime();
        }

        /// <summary>
        /// 以相对纪元的时间间隔设置结束时间
        /// </summary>
        /// <param name="value">结束时间相对纪元的偏移量</param>
        public void SetEndSpan(TimeSpan? value)
        {
            if (value == null) return;
            SetEndTime(Epoch.Add(value.Value));
        }

        /// <summary>
        /// 按日期类型对应的默认格式输出日期范围字符串
        /// </summary>
        /// <returns>格式化后的日期范围字符串，开始或结束时间未设置时返回空字符串</returns>
        public override string ToString()
        {
            return ToString(DateTimeFormatDic[Type], "~");
        }

        /// <summary>
        /// 按指定格式输出日期范围字符串
        /// </summary>
        /// <param name="format">日期时间输出格式</param>
        /// <returns>格式化后的日期范围字符串</returns>
        public string ToString(string format)
        {
            return ToString(format, "~");
        }

        /// <summary>
        /// 按指定格式和分隔符输出日期范围字符串
        /// </summary>
        /// <param name="format">日期时间输出格式</param>
        /// <param name="rangeSplit">开始时间与结束时间之间的分隔符</param>
        /// <returns>格式化后的日期范围字符串，开始或结束时间未设置时返回空字符串</returns>
        public string ToString(string format, string rangeSplit)
        {
            if (_startTime.HasValue && _endTime.HasValue)
                return $"{_startTime?.ToString(format)} {rangeSplit} {_endTime?.ToString(format)}";
            return string.Empty;
        }

        /// <summary>
        /// 各日期类型对应的默认输出格式字典
        /// </summary>
        public static readonly Dictionary<DateTimeTypeEnum, string> DateTimeFormatDic = new Dictionary<DateTimeTypeEnum, string>()
        {
            { DateTimeTypeEnum.Date,"yyyy-MM-dd"},
            { DateTimeTypeEnum.DateTime,"yyyy-MM-dd HH:mm:ss"},
            { DateTimeTypeEnum.Year,"yyyy"},
            { DateTimeTypeEnum.Month,"yyyy-MM"},
            { DateTimeTypeEnum.Time,"HH:mm:ss"},
        };

        /// <summary>
        /// 将给定时间的DateTimeKind转换成与纪元一致的类型，必要时在本地时间、UTC时间和未指定类型之间换算
        /// </summary>
        /// <param name="time">待转换的时间</param>
        /// <returns>DateTimeKind与纪元一致的时间</returns>
        private DateTime SwitchTime(DateTime time)
        {
            if (Epoch.Kind == time.Kind)
                return time;
            switch (Epoch.Kind)
            {
                case DateTimeKind.Local:
                    switch (time.Kind)
                    {
                        case DateTimeKind.Unspecified:
                            return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Local);
                        case DateTimeKind.Utc:
                            return time.ToLocalTime();
                    }
                    break;
                case DateTimeKind.Unspecified:
                    switch (time.Kind)
                    {
                        case DateTimeKind.Local:
                            return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Unspecified);
                        case DateTimeKind.Utc:
                            return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Unspecified);
                    }
                    break;
                case DateTimeKind.Utc:
                    switch (time.Kind)
                    {
                        case DateTimeKind.Local:
                            return time.ToUniversalTime();
                        case DateTimeKind.Unspecified:
                            return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Utc);
                    }
                    break;
            }
            return time;
        }

        /// <summary>
        /// 默认日期范围，即今天
        /// </summary>
        public static DateRange Default => Today;

        /// <summary>
        /// 最近90天(本地时间)的日期范围
        /// </summary>
        public static DateRange NinetyDays
        {
            get
            {
                var result = new DateRange(DateTime.Today.AddDays(-90), DateTime.Today);
                return result;
            }
        }

        /// <summary>
        /// 最近30天(本地时间)的日期范围
        /// </summary>
        public static DateRange ThirtyDays
        {
            get
            {
                var result = new DateRange(DateTime.Today.AddDays(-30), DateTime.Today);
                return result;
            }
        }

        /// <summary>
        /// 最近两周(本地时间)的日期范围
        /// </summary>
        public static DateRange TwoWeek
        {
            get
            {
                var result = new DateRange(DateTime.Today.AddDays(-14), DateTime.Today);
                return result;
            }
        }

        /// <summary>
        /// 最近一周(本地时间)的日期范围
        /// </summary>
        public static DateRange Week
        {
            get
            {
                var result = new DateRange(DateTime.Today.AddDays(-7), DateTime.Today);
                return result;
            }
        }

        /// <summary>
        /// 今天(本地时间)的日期范围
        /// </summary>
        public static DateRange Today
        {
            get
            {
                var result = new DateRange(DateTime.Today, DateTime.Today);
                return result;
            }
        }


        /// <summary>
        /// 昨天(本地时间)的日期范围
        /// </summary>
        public static DateRange Yesterday
        {
            get
            {
                var result = new DateRange(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1));
                return result;
            }
        }


        /// <summary>
        /// 默认UTC日期范围，即UTC今天
        /// </summary>
        public static DateRange UtcDefault => UtcToday;

        /// <summary>
        /// UTC时间最近90天的日期范围
        /// </summary>
        public static DateRange UtcNinetyDays
        {
            get
            {
                var result = new DateRange(DateTime.UtcNow.Date.AddDays(-90), DateTime.UtcNow.Date);
                return result;
            }
        }

        /// <summary>
        /// UTC时间最近30天的日期范围
        /// </summary>
        public static DateRange UtcThirtyDays
        {
            get
            {
                var result = new DateRange(DateTime.UtcNow.Date.AddDays(-30), DateTime.UtcNow.Date, DefaultType, UtCDefaultEpoch);
                return result;
            }
        }

        /// <summary>
        /// UTC时间最近两周的日期范围
        /// </summary>
        public static DateRange UtcTwoWeek
        {
            get
            {
                var result = new DateRange(DateTime.UtcNow.Date.AddDays(-14), DateTime.UtcNow.Date, DefaultType, UtCDefaultEpoch);
                return result;
            }
        }

        /// <summary>
        /// UTC时间最近一周的日期范围
        /// </summary>
        public static DateRange UtcWeek
        {
            get
            {
                var result = new DateRange(DateTime.UtcNow.Date.AddDays(-7), DateTime.UtcNow.Date, DefaultType, UtCDefaultEpoch);
                return result;
            }

        }

        /// <summary>
        /// UTC时间当天的日期范围
        /// </summary>
        public static DateRange UtcToday
        {
            get
            {
                var result = new DateRange(DateTime.UtcNow.Date.AddDays(-1), DateTime.UtcNow.Date, DefaultType, UtCDefaultEpoch);
                return result;
            }
        }

        /// <summary>
        /// UTC时间昨天的日期范围
        /// </summary>
        public static DateRange UtcYesterday
        {
            get
            {
                var result = new DateRange(DateTime.UtcNow.Date.AddDays(-1), DateTime.UtcNow.Date, DefaultType, UtCDefaultEpoch);
                return result;
            }
        }


        //各日期类型用于从字符串中提取日期时间的正则表达式字典
        private static readonly Dictionary<DateTimeTypeEnum, string> DateTimeRegexDic = new Dictionary<DateTimeTypeEnum, string>()
        {
            { DateTimeTypeEnum.DateTime,@"((\d{4}|\d{3}|\d{2}|\d{1})[-](1[0-2]|0?[1-9])[-](3[01]|[12][0-9]|0?[1-9])\s+(20|21|22|23|[0-1]\d):[0-5]\d:[0-5]\d)"},
            { DateTimeTypeEnum.Date,@"(\d{4}|\d{3}|\d{2}|\d{1})[-](1[0-2]|0?[1-9])[-](3[01]|[12][0-9]|0?[1-9])"},
            { DateTimeTypeEnum.Month,@"(\d{4}|\d{3}|\d{2}|\d{1})[-](1[0-2]|0?[1-9])"},
            { DateTimeTypeEnum.Time,@"(20|21|22|23|[0-1]\d):[0-5]\d:[0-5]\d"},
            { DateTimeTypeEnum.Year,@"(\d{4}|\d{3}|\d{2}|\d{1})"},
        };

        /// <summary>
        /// 尝试将字符串解析为日期范围：先按~分隔符解析，失败后再按各日期类型的正则表达式提取起止时间
        /// </summary>
        /// <param name="input">待解析的字符串</param>
        /// <param name="result">解析成功时输出的日期范围，失败时为null</param>
        /// <returns>解析成功返回true，否则返回false</returns>
        public static bool TryParse(string input, out DateRange result)
        {
            if (TryParse(input, new[] { '~' }, out result)) return true;
            result = null;
            foreach (var pair in DateTimeRegexDic)
            {
                if (Regex.IsMatch(input, pair.Value))
                {
                    var values = Regex.Matches(input, pair.Value, RegexOptions.IgnorePatternWhitespace);
                    return values.Count == 2 && TryParse(values[0].Value, values[1].Value, DefaultEpoch, out result);
                }
            }
            return false;
        }

        /// <summary>
        /// 尝试按指定的字符分隔符将字符串解析为日期范围
        /// </summary>
        /// <param name="input">待解析的字符串</param>
        /// <param name="separator">开始时间与结束时间之间的分隔字符数组</param>
        /// <param name="result">解析成功时输出的日期范围，失败时为null</param>
        /// <returns>解析成功返回true，否则返回false</returns>
        public static bool TryParse(string input, char[] separator, out DateRange result)
        {
            result = null;
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            var values = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            return values.Length == 2 && TryParse(values[0], values[1], DefaultEpoch, out result);
        }

        /// <summary>
        /// 尝试按指定的字符串分隔符将字符串解析为日期范围
        /// </summary>
        /// <param name="input">待解析的字符串</param>
        /// <param name="separator">开始时间与结束时间之间的分隔字符串数组</param>
        /// <param name="result">解析成功时输出的日期范围，失败时为null</param>
        /// <returns>解析成功返回true，否则返回false</returns>
        public static bool TryParse(string input, string[] separator, out DateRange result)
        {
            result = null;
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            var values = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            return values.Length == 2 && TryParse(values[0], values[1], DefaultEpoch, out result);
        }

        /// <summary>
        /// 尝试将包含开始时间和结束时间的字符串数组解析为日期范围
        /// </summary>
        /// <param name="input">包含开始时间和结束时间两个元素的字符串数组</param>
        /// <param name="result">解析成功时输出的日期范围，失败时为null</param>
        /// <returns>解析成功返回true，否则返回false</returns>
        public static bool TryParse(string[] input, out DateRange result)
        {
            result = null;
            return input.Length == 2 && TryParse(input[0], input[1], DefaultEpoch, out result);
        }


        /// <summary>
        /// 尝试将开始时间和结束时间的字符串表示解析为日期范围，按开始时间字符串的长度自动识别年、月、时间、日期和日期时间类型
        /// </summary>
        /// <param name="startTime">开始时间字符串</param>
        /// <param name="endTime">结束时间字符串</param>
        /// <param name="epoch">时间戳纪元基准时间</param>
        /// <param name="result">解析成功时输出的日期范围，失败时为null</param>
        /// <returns>解析成功返回true，否则返回false</returns>
        public static bool TryParse(string startTime, string endTime, DateTime epoch, out DateRange result)
        {
            result = null;
            switch (startTime.Trim().Length)
            {
                //Year
                case 4:
                    {
                        if (!int.TryParse(startTime, out var y1)) return false;
                        if (y1 < 1 || y1 > 9999) return false;

                        if (!int.TryParse(endTime, out var y2)) return false;
                        if (y2 < 1 || y2 > 9999) return false;

                        result = new DateRange(new DateTime(y1, 1, 1, 0, 0, 0, epoch.Kind), new DateTime(y2, 1, 1, 0, 0, 0, epoch.Kind));
                        return true;

                    }
                //Month
                case 7:
                    {
                        if (!DateTime.TryParse(startTime, out var v1)) return false;

                        if (!DateTime.TryParse(endTime, out var v2)) return false;

                        result = new DateRange(v1, v2, DateTimeTypeEnum.Month, epoch);

                        return true;
                    }
                //Time
                case 8:
                    {
                        if (!DateTime.TryParse(startTime, out var v1)) return false;

                        if (!DateTime.TryParse(endTime, out var v2)) return false;

                        result = new DateRange(v1, v2, DateTimeTypeEnum.Time, epoch);

                        return true;
                    }
                //Date
                case 10:
                    {
                        if (!DateTime.TryParse(startTime, out var v1)) return false;

                        if (!DateTime.TryParse(endTime, out var v2)) return false;

                        result = new DateRange(v1, v2, DateTimeTypeEnum.Date, epoch);

                        return true;
                    }
                //DateTime
                default:
                    {
                        if (!DateTime.TryParse(startTime, out var v1)) return false;

                        if (!DateTime.TryParse(endTime, out var v2)) return false;

                        result = new DateRange(v1, v2, DateTimeTypeEnum.DateTime, epoch);

                        return true;
                    }
            }
        }
    }
}

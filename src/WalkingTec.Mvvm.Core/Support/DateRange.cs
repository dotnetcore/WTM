using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WalkingTec.Mvvm.Core
{
    public sealed class DateRange
    {
        private static readonly DateTime UtCDefaultEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime DefaultEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        private static readonly DateTimeTypeEnum DefaultType = DateTimeTypeEnum.DateTime;


        private DateRange() { }


        public DateRange(DateTime startTime, DateTime endTime) : this(startTime, endTime, DefaultType, DefaultEpoch) { }

        public DateRange(DateTime startTime, DateTime endTime, DateTimeTypeEnum type, DateTime epoch)
        {
            Type = type;
            Epoch = epoch;
            SetStartTime(startTime);
            SetEndTime(endTime);
        }


        public DateRange(TimeSpan startSpan, TimeSpan endSpan) : this(startSpan, endSpan, DefaultType, DefaultEpoch) { }

        public DateRange(TimeSpan startSpan, TimeSpan endSpan, DateTimeTypeEnum type, DateTime epoch)
        {
            Type = type;
            Epoch = epoch;
            SetStartTime(epoch.Add(startSpan));
            SetEndTime(epoch.Add(endSpan));
        }

        public DateRange(DateTimeOffset startOffset, DateTimeOffset endOffset) : this(startOffset, endOffset, DefaultType, DefaultEpoch) { }

        public DateRange(DateTimeOffset startOffset, DateTimeOffset endOffset, DateTimeTypeEnum type, DateTime epoch)
        {
            Type = type;
            Epoch = epoch;
            SetStartTime(startOffset.DateTime);
            SetEndTime(endOffset.DateTime);
        }

        public readonly DateTime Epoch = DefaultEpoch;

        public readonly DateTimeTypeEnum Type = DefaultType;


        public string Value => ToString();


        private DateTime? _startTime;

        public DateTime? GetStartTime()
        {
            return _startTime;
        }

        public void SetStartTime(DateTime? value)
        {
            if (value == null) return;

            value = SwitchTime(value.Value);

            if (_endTime.HasValue && _endTime.Value < value) return;
            _startTime = value;
        }

        private DateTime? _endTime;

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

        public void SetEndTime(DateTime? value)
        {
            if (value == null) return;

            value = SwitchTime(value.Value);

            if (_startTime.HasValue && _startTime.Value > value) return;
            _endTime = value;
        }


        public DateTimeOffset? GetStartOffset()
        {
            if (_startTime == null)
                return null;
            return _startTime <= DateTimeOffset.MinValue.LocalDateTime ? DateTimeOffset.MinValue : new DateTimeOffset(GetStartTime().Value);
        }

        public void SetStartOffset(DateTimeOffset? value)
        {
            if (value == null) return;
            SetStartTime(value.Value.LocalDateTime);
        }

        public DateTimeOffset? GetEndOffset()
        {
            if (_endTime == null)
                return null;
            return _endTime <= DateTimeOffset.MinValue.LocalDateTime ? DateTimeOffset.MinValue : new DateTimeOffset(GetEndTime().Value);
        }

        public void SetEndOffset(DateTimeOffset? value)
        {
            if (value == null) return;
            SetEndTime(value.Value.LocalDateTime);
        }


        public TimeSpan? GetStartSpan()
        {
            if (_endTime == null)
                return null;
            return GetStartTime() - Epoch.ToLocalTime();
        }

        public void SetStartSpan(TimeSpan? value)
        {
            if (value == null) return;
            SetStartTime(Epoch.Add(value.Value));
        }

        public TimeSpan? GetEndSpan()
        {
            if (_endTime == null)
                return null;
            return GetEndTime() - Epoch.ToLocalTime();
        }

        public void SetEndSpan(TimeSpan? value)
        {
            if (value == null) return;
            SetEndTime(Epoch.Add(value.Value));
        }

        public override string ToString()
        {
            return ToString(DateTimeFormatDic[Type], "~");
        }

        public string ToString(string format)
        {
            return ToString(format, "~");
        }

        public string ToString(string format, string rangeSplit)
        {
            if (_startTime.HasValue && _endTime.HasValue)
                return $"{_startTime?.ToString(format)} {rangeSplit} {_endTime?.ToString(format)}";
            return string.Empty;
        }

        public static readonly Dictionary<DateTimeTypeEnum, string> DateTimeFormatDic = new Dictionary<DateTimeTypeEnum, string>()
        {
            { DateTimeTypeEnum.Date,"yyyy-MM-dd"},
            { DateTimeTypeEnum.DateTime,"yyyy-MM-dd HH:mm:ss"},
            { DateTimeTypeEnum.Year,"yyyy"},
            { DateTimeTypeEnum.Month,"yyyy-MM"},
            { DateTimeTypeEnum.Time,"HH:mm:ss"},
        };

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

        public static DateRange Default => Today;

        public static DateRange NinetyDays
        {
            get
            {
                var result = new DateRange(DateTime.Today.AddDays(-90), DateTime.Today);
                return result;
            }
        }

        public static DateRange ThirtyDays
        {
            get
            {
                var result = new DateRange(DateTime.Today.AddDays(-30), DateTime.Today);
                return result;
            }
        }

        public static DateRange TwoWeek
        {
            get
            {
                var result = new DateRange(DateTime.Today.AddDays(-14), DateTime.Today);
                return result;
            }
        }

        public static DateRange Week
        {
            get
            {
                var result = new DateRange(DateTime.Today.AddDays(-7), DateTime.Today);
                return result;
            }
        }

        public static DateRange Today
        {
            get
            {
                var result = new DateRange(DateTime.Today, DateTime.Today);
                return result;
            }
        }


        public static DateRange Yesterday
        {
            get
            {
                var result = new DateRange(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1));
                return result;
            }
        }


        public static DateRange UtcDefault => UtcToday;

        public static DateRange UtcNinetyDays
        {
            get
            {
                var result = new DateRange(DateTime.UtcNow.Date.AddDays(-90), DateTime.UtcNow.Date);
                return result;
            }
        }

        public static DateRange UtcThirtyDays
        {
            get
            {
                var result = new DateRange(DateTime.UtcNow.Date.AddDays(-30), DateTime.UtcNow.Date, DefaultType, UtCDefaultEpoch);
                return result;
            }
        }

        public static DateRange UtcTwoWeek
        {
            get
            {
                var result = new DateRange(DateTime.UtcNow.Date.AddDays(-14), DateTime.UtcNow.Date, DefaultType, UtCDefaultEpoch);
                return result;
            }
        }

        public static DateRange UtcWeek
        {
            get
            {
                var result = new DateRange(DateTime.UtcNow.Date.AddDays(-7), DateTime.UtcNow.Date, DefaultType, UtCDefaultEpoch);
                return result;
            }

        }

        public static DateRange UtcToday
        {
            get
            {
                var result = new DateRange(DateTime.UtcNow.Date.AddDays(-1), DateTime.UtcNow.Date, DefaultType, UtCDefaultEpoch);
                return result;
            }
        }

        public static DateRange UtcYesterday
        {
            get
            {
                var result = new DateRange(DateTime.UtcNow.Date.AddDays(-1), DateTime.UtcNow.Date, DefaultType, UtCDefaultEpoch);
                return result;
            }
        }


        private static readonly Dictionary<DateTimeTypeEnum, string> DateTimeRegexDic = new Dictionary<DateTimeTypeEnum, string>()
        {
            { DateTimeTypeEnum.DateTime,@"((\d{4}|\d{3}|\d{2}|\d{1})[-](1[0-2]|0?[1-9])[-](3[01]|[12][0-9]|0?[1-9])\s+(20|21|22|23|[0-1]\d):[0-5]\d:[0-5]\d)"},
            { DateTimeTypeEnum.Date,@"(\d{4}|\d{3}|\d{2}|\d{1})[-](1[0-2]|0?[1-9])[-](3[01]|[12][0-9]|0?[1-9])"},
            { DateTimeTypeEnum.Month,@"(\d{4}|\d{3}|\d{2}|\d{1})[-](1[0-2]|0?[1-9])"},
            { DateTimeTypeEnum.Time,@"(20|21|22|23|[0-1]\d):[0-5]\d:[0-5]\d"},
            { DateTimeTypeEnum.Year,@"(\d{4}|\d{3}|\d{2}|\d{1})"},
        };

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

        public static bool TryParse(string[] input, out DateRange result)
        {
            result = null;
            return input.Length == 2 && TryParse(input[0], input[1], DefaultEpoch, out result);
        }


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

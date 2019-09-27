using System;
using System.Collections.Generic;

namespace WalkingTec.Mvvm.Core
{
    public sealed class DateRange
    {
        private static readonly DateTime UtCDefaultEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime DefaultEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        private readonly DateTime _epoch = DefaultEpoch;

        private readonly DateTimeKind _kind = DateTimeKind.Local;

        private readonly DateTimeTypeEnum _type = DateTimeTypeEnum.Date;


        private DateRange() { }


        public DateRange(DateTime startTime, DateTime endTime)
        {
            SetStartTime(startTime);
            SetEndTime(endTime);
        }


        public DateRange(TimeSpan startSpan, TimeSpan endSpan) : this(startSpan, endSpan, DefaultEpoch) { }

        public DateRange(TimeSpan startSpan, TimeSpan endSpan, DateTime epoch)
        {
            _epoch = epoch;
            _kind = epoch.Kind;
            SetStartTime(epoch.Add(startSpan));
            SetEndTime(epoch.Add(endSpan));
        }

        public DateRange(DateTimeOffset startOffset, DateTimeOffset endOffset)
        {
            SetStartTime(startOffset.DateTime);
            SetEndTime(endOffset.DateTime);
        }


        public DateRange(string dateRange)
        {
            Value = dateRange;
        }

        public string Value
        {
            get => ToString();
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }
                var values = value.Split(new[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length != 2) return;

                if (DateTime.TryParse(values[0], out var v1))
                {
                    SetStartTime(v1);
                }

                if (!DateTime.TryParse(values[1], out var v2)) return;
                SetEndTime(v2);
            }
        }


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
            return _endTime;
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
            return _startTime <= DateTimeOffset.MinValue.LocalDateTime ? DateTimeOffset.MinValue : new DateTimeOffset(_startTime.Value);
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
            return _endTime <= DateTimeOffset.MinValue.LocalDateTime ? DateTimeOffset.MinValue : new DateTimeOffset(_endTime.Value);
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
            return _endTime - _epoch.ToLocalTime();
        }

        public void SetStartSpan(TimeSpan? value)
        {
            if (value == null) return;
            SetStartTime(_epoch.Add(value.Value));
        }

        public TimeSpan? GetEndSpan()
        {
            if (_endTime == null)
                return null;
            return _startTime - _epoch.ToLocalTime();
        }

        public void SetEndSpan(TimeSpan? value)
        {
            if (value == null) return;
            SetEndTime(_epoch.Add(value.Value));
        }

        public string ToString(string format)
        {
            if (_startTime.HasValue && _endTime.HasValue)
                return $"{_startTime?.ToString(format)} ~ {_endTime?.ToString(format)}";
            return string.Empty;
        }

        public override string ToString()
        {
            if (_startTime.HasValue && _endTime.HasValue)
                return $"{_startTime?.ToString(DateTimeFormatDic[_type])} ~ {_endTime?.ToString(DateTimeFormatDic[_type])}";
            return string.Empty;
        }

        public static readonly Dictionary<DateTimeTypeEnum, string> DateTimeFormatDic = new Dictionary<DateTimeTypeEnum, string>()
        {
            { DateTimeTypeEnum.Date,"yyyy-MM-dd"},
            { DateTimeTypeEnum.Datetime,"yyyy-MM-dd HH:mm:ss"},
            { DateTimeTypeEnum.Year,"yyyy"},
            { DateTimeTypeEnum.Month,"yyyy-MM"},
            { DateTimeTypeEnum.Time,"HH:mm:ss"},
        };

        private DateTime SwitchTime(DateTime time)
        {
            if (_kind == time.Kind)
                return time;
            switch (_kind)
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

        public static DateRange Default
        {
            get
            {
                var result = new DateRange();
                result.SetStartTime(DateTime.Today.AddDays(-1));
                result.SetEndTime(DateTime.Today);
                return result;
            }
        }

        public static DateRange UtCDefault
        {
            get
            {
                var result = new DateRange();
                result.SetStartTime(DateTime.UtcNow.Date.AddDays(-1));
                result.SetEndTime(DateTime.UtcNow.Date);
                return result;
            }
        }

        public static bool TryParse(string input, out DateRange result)
        {
            result = null;
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            var values = input.Split(new[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
            if (values.Length != 2) return false;

            switch (values[0].Trim().Length)
            {
                //Year
                case 4:
                    {
                        if (!int.TryParse(values[0], out var y1)) return false;
                        if (y1 < 1 || y1 > 9999) return false;

                        if (!int.TryParse(values[1], out var y2)) return false;
                        if (y2 < 1 || y2 > 9999) return false;

                        result = new DateRange();
                        result.SetStartTime(new DateTime(y1, 1, 1));
                        result.SetEndTime(new DateTime(y2, 1, 1));
                        return true;

                    }
                //Month
                case 7:
                //Time
                case 8:
                //Date
                case 10:
                //Datetime
                default:
                    if (!DateTime.TryParse(values[0], out var v1)) return false;

                    if (!DateTime.TryParse(values[1], out var v2)) return false;

                    result = new DateRange();
                    result.SetStartTime(v1);
                    result.SetEndTime(v2);

                    return true;
            }
        }
    }
}

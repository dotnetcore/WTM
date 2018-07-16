using System;

namespace WalkingTec.Mvvm.Core.Extensions
{
    /// <summary>
    /// DateTime Helper
    /// </summary>
    public static class DateTimeHelper
    {
        #region DateTime Helper

        public static int WeekOfYear(this DateTime self)
        {
            var startDayOfYear  = new DateTime(self.Year,1,1);
            var weekOffset = 7 - ( startDayOfYear.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)startDayOfYear.DayOfWeek) + 1;
            var weekOfYear = (int)Math.Ceiling((self.DayOfYear - weekOffset) / 7.0 + (weekOffset == 0 ? 0 : 1));

            return weekOfYear;
        }

        /// <summary>
        /// 获取 指定的一周所在年份 的开始及结束时间
        /// </summary>
        /// <param name="yearNum">所在年份</param>
        /// <param name="weekOfYear">周数</param>
        /// <param name="startDay">指定周开始时间</param>
        /// <param name="endDay">指定周结束时间</param>
        public static void WeekDays(int yearNum, int weekOfYear, out DateTime startDay, out DateTime endDay)
        {
            var startDayOfYear  = new DateTime(yearNum,1,1,0,0,0);

            var weekOffset = 7 - ( startDayOfYear.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)startDayOfYear.DayOfWeek) + 1;
            startDay = startDayOfYear.AddDays(7 * (weekOfYear - (weekOffset == 0 ? 0 : 1)) + weekOffset - 7);
            endDay = startDay.AddDays(7);
        }

        #endregion

        #region DateTime Extensions

        private static readonly DateTime _jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0,DateTimeKind.Utc);

        /// <summary>
        /// UTC 1970/01/01 00:00:00
        /// </summary>
        public static DateTime Jan1st1970 => _jan1st1970;

        /// <summary>
        /// 时间戳 ms
        /// </summary>
        /// <param name="self"></param>
        /// <returns>返回标准时间戳 单位 毫秒 注：从 1970/01/01 00:00:00 开始</returns>
        public static long ToMilliseconds(this DateTime self)
        {
            return (long)(self.ToUniversalTime() - Jan1st1970).TotalMilliseconds;
        }

        /// <summary>
        /// 时间戳 microsecond
        /// </summary>
        /// <param name="self"></param>
        /// <returns>返回标准时间戳 单位 微秒 注：从 1970/01/01 00:00:00 开始</returns>
        public static long ToMicroseconds(this DateTime self)
        {
            return (long)((self.ToUniversalTime() - Jan1st1970).TotalMilliseconds * 1000);
        }

        /// <summary>
        /// 获取当前时间所在周的开始及结束时间
        /// </summary>
        /// <param name="self"></param>
        /// <param name="startDay">指定周开始时间</param>
        /// <param name="endDay">指定周结束时间</param>
        public static void WeekDays(this DateTime self, out DateTime startDay, out DateTime endDay)
        {
            WeekDays(self.Year, self.WeekOfYear(), out startDay, out endDay);
        }

        #endregion
    }
}

using System;
using TimeZoneConverter;

namespace Lingua.Shared
{
    public static class Utilities
    {
        public static DateTime ConvertToTimezone(DateTime utc, string timezone)
        {
            var tz = TZConvert.GetTimeZoneInfo(timezone);
            return TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
        }
    }
}

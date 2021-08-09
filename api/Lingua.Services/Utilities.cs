using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace Lingua.Services
{
    public static class Utilities
    {
        public static DateTime ConvertToTimezone(DateTime utc, string timezone)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(TZConvert.IanaToWindows(timezone));
            return TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
        }
    }
}

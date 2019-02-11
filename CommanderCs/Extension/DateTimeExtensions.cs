using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Extension
{
    static class DateTimeExtensions
    {
        public static string Format(this DateTime dateTime)
        {
            var jsDataTime = (dateTime.ToUniversalTime().Ticks - dateTimeMinTicks) / 10000;
            return jsDataTime.ToString();
        }

        static readonly long dateTimeMinTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
    }
}

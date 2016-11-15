using System;

namespace Framework.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToShortDateString(this DateTime date)
        {
            return date.ToString("d");
        }
    }
}

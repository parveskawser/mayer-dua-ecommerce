using System;

namespace MDUA.Web.UI.Extensions
{
    public static class DateExtensions
    {
        public static string ToUtcString(this DateTime? date)
        {
            if (!date.HasValue) return "";
            return date.Value.ToString("yyyy-MM-ddTHH:mm:ss") + "Z";
        }
        public static string ToUtcString(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ss") + "Z";
        }
    }
}
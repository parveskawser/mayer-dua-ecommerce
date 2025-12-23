using Microsoft.AspNetCore.Http;
using MDUA.Framework;
using System.Globalization;

namespace MDUA
{
    public static class DateTimeExtension
    {
        private static HttpContext _httpContext => new HttpContextAccessor().HttpContext;
        public static DateTime ToUtc(this DateTime dt)
        {
            var CurrentZone = "";
            double hourstocount = 0.00;
            var CookieCurrentUserTime = _httpContext.Request.Cookies[CookieKeys.MyTime];
            if (CookieCurrentUserTime != null && dt != new DateTime())
            {
                CurrentZone = CookieCurrentUserTime.ToString().Replace(":", ".");
                hourstocount = (60 * double.Parse(CurrentZone)) * -1;
                dt = dt.AddMinutes(hourstocount);
            }
            return dt;
        }
        public static string ToUtc(this string dt, bool isMax = false)
        {
            var CurrentZone = "";
            double hourstocount = 0.00;
            var CookieCurrentUserTime = _httpContext.Request.Cookies[CookieKeys.MyTime];
            if (CookieCurrentUserTime != null && dt != "")
            {
                CurrentZone = CookieCurrentUserTime.ToString().Replace(":", ".");
                hourstocount = double.Parse(CurrentZone);
                DateTime dtLocal = DateTime.Parse(dt).SetZeroHour();
                if (isMax)
                {
                    dtLocal = DateTime.Parse(dt).SetMaxHour();
                }
                dt = dtLocal.AddHours(-hourstocount).ToString("yyyy-MM-dd HH:mm:ss");
            }
            return dt;
        }
        public static DateTime ToUtcWithMyTime(this DateTime dt, string MyTime, bool isMax = false)
        {
            var CurrentZone = "";
            double hourstocount = 0.00;
            var CookieCurrentUserTime = MyTime;
            if (CookieCurrentUserTime != null && dt != new DateTime())
            {
                CurrentZone = CookieCurrentUserTime.ToString().Replace(":", ".");
                hourstocount = (60 * double.Parse(CurrentZone)) * -1;
                dt = dt.AddMinutes(hourstocount);
            }
            return dt;
        }
        public static DateTime ClientToUTCTime(this DateTime datetime, string CookieCurrentUser)
        {
            var CurrentTimeZone = "";
            var CurrentDate = "";
            var CurrentTime = "";
            TimeZoneInfo currentTimeZone = TimeZoneInfo.Local;
            DateTime LocalTime = new DateTime();
            //string CookieCurrentUser = HttpContext.Current.Request.Cookies[CookieKeys.CurrentUser].Value;
            if (!string.IsNullOrWhiteSpace(CookieCurrentUser))
            {

                bool isplus = false;
                if (CookieCurrentUser.IndexOf("+") > -1)
                {
                    isplus = true;
                }
                int hours = 0;
                int minutes = 0;
                string[] timestamp = CookieCurrentUser.Split(':');
                hours = int.Parse(timestamp[0]);
                minutes = int.Parse(timestamp[1]);
                datetime = datetime.AddHours(hours);
                if (!isplus)
                    minutes = -1 * minutes;
                datetime = datetime.AddMinutes(minutes);
                //+ Kina 
                ///AddMinutes
                ///

                //if - 
                // AddMinutes
                /* string[] splitCurrentUser = CookieCurrentUser.Split('&');
                 CurrentTimeZone = splitCurrentUser[0];
                 var ctz = CurrentTimeZone.Split('=');
                 CurrentDate = splitCurrentUser[1];
                 CurrentTime = splitCurrentUser[2];
                 var TimeZoneId = ctz[1];
                 var ctzid = splitCurrentUser[3].Split('=');
                 var Hourstocount = Convert.ToInt32(TimeZoneId);
                 datetime = datetime.AddMinutes(Hourstocount);*/
                //datetime.ToUniversalTime();
            }

            return datetime;
        }

        //public static DateTime GetLocalTime(this DateTime datetime)
        //{ 
        //    var CookieCurrentUser = HttpContext.Current.Request.Cookies[CookieKeys.CurrentUser].Value;
        //    if (!string.IsNullOrWhiteSpace(CookieCurrentUser))
        //    {
        //        string[] splitCurrentUser = CookieCurrentUser.Split('&');
        //        string CurrentTimeZone = splitCurrentUser[0];
        //        var ctz = CurrentTimeZone.Split('=');
        //        string CurrentDate = splitCurrentUser[1];
        //        string CurrentTime = splitCurrentUser[2];
        //        var TimeZoneId = ctz[1];
        //        var ctzid = splitCurrentUser[3].Split('=');
        //        var Hourstocount = Convert.ToInt32(TimeZoneId) * -1;
        //        datetime = DateTime.UtcNow.AddMinutes(Hourstocount);
        //    }
        //    return datetime;
        //}
        public static DateTime ToClientTime(this DateTime datetime)
        {
            var CurrentZone = "";
            double hourstocount = 0.00;
            var CookieCurrentUserTime = _httpContext.Request.Cookies[CookieKeys.MyTime];
            if (CookieCurrentUserTime != null && datetime != new DateTime())
            {
                CurrentZone = CookieCurrentUserTime.ToString().Replace(":", ".");
                hourstocount = 60 * double.Parse(CurrentZone);
                datetime = datetime.AddMinutes(hourstocount);
            }
            return datetime;
        }
        public static double ClientZone()
        {
            var CurrentZone = "";
            double hourstocount = 0.00;
            var CookieCurrentUserTime = _httpContext.Request.Cookies[CookieKeys.MyTime];
            if (CookieCurrentUserTime != null)
            {
                CurrentZone = CookieCurrentUserTime.ToString().Replace(":", ".");
                hourstocount = double.Parse(CurrentZone);
            }
            return hourstocount;
        }
        public static DateTime UTCToClientTime(this DateTime datetime)
        {
            var CurrentTimeZone = "";
            try
            {
                var CookieCurrentUser = _httpContext.Request.Cookies[CookieKeys.MyTime];
                if (CookieCurrentUser != null && datetime != new DateTime())
                {
                    CurrentTimeZone = CookieCurrentUser.ToString().Replace(":", ".");
                    var Hourstocount = Convert.ToDouble(CurrentTimeZone);
                    datetime = datetime.AddHours(Hourstocount);
                }

            }
            catch (Exception e)
            {
                return datetime;
            }


            return datetime;
        }


        public static DateTime UTCToClientTime(this DateTime datetime, string utc)
        {
            var CurrentTimeZone = "";
            var CurrentDate = "";
            var CurrentTime = "";
            TimeZoneInfo currentTimeZone = TimeZoneInfo.Local;
            if (!string.IsNullOrWhiteSpace(utc))
            {
                CurrentTimeZone = utc.ToString().Replace(":", ".");
                var Hourstocount = Convert.ToDouble(CurrentTimeZone);
                datetime = datetime.AddHours(Hourstocount);
            }
            return datetime;
        }

        public static DateTime UTCToServerTime(this DateTime dateTime)
        {
            DateTime currentDate = DateTime.Now;
            TimeZone localZone = TimeZone.CurrentTimeZone;
            TimeSpan currentOffset = localZone.GetUtcOffset(currentDate);

            var Hourstocount = Convert.ToInt32(currentOffset.Hours * 60 + currentOffset.Minutes) * -1;
            dateTime = dateTime.AddMinutes(Hourstocount);

            return dateTime;
        }
        public static DateTime StartOfWeek(this DateTime dt)
        {
            DateTime startweek = new DateTime();
            if (dt != null)
            {
                int weekday = dt.Day;
                string strDayOfWeek = dt.DayOfWeek.ToString();
                if (strDayOfWeek.ToLower() == "monday")
                {
                    weekday = weekday - 1;
                    int weekmonth = dt.Month;
                    int weekyear = dt.Year;
                    startweek = new DateTime(weekyear, weekmonth, weekday);
                }
                else if (strDayOfWeek.ToLower() == "tuesday")
                {
                    weekday = weekday - 2;
                    int weekmonth = dt.Month;
                    int weekyear = dt.Year;
                    startweek = new DateTime(weekyear, weekmonth, weekday);
                }
                else if (strDayOfWeek.ToLower() == "wednesday")
                {
                    weekday = weekday - 3;
                    int weekmonth = dt.Month;
                    int weekyear = dt.Year;
                    startweek = new DateTime(weekyear, weekmonth, weekday);
                }
                else if (strDayOfWeek.ToLower() == "thursday")
                {
                    weekday = weekday - 4;
                    int weekmonth = dt.Month;
                    int weekyear = dt.Year;
                    startweek = new DateTime(weekyear, weekmonth, weekday);
                }
                else if (strDayOfWeek.ToLower() == "friday")
                {
                    weekday = weekday - 5;
                    int weekmonth = dt.Month;
                    int weekyear = dt.Year;
                    startweek = new DateTime(weekyear, weekmonth, weekday);
                }
                else if (strDayOfWeek.ToLower() == "saturday")
                {
                    weekday = weekday - 6;
                    int weekmonth = dt.Month;
                    int weekyear = dt.Year;
                    startweek = new DateTime(weekyear, weekmonth, weekday);
                }
                else
                {
                    int weekmonth = dt.Month;
                    int weekyear = dt.Year;
                    startweek = new DateTime(weekyear, weekmonth, weekday);
                }
            }
            return startweek;
        }

        public static DateTime GetFirstDayOfWeek(this DateTime date)
        {
            var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;

            while (date.DayOfWeek != firstDayOfWeek)
            {
                date = date.AddDays(-1);
            }

            return date;
        }

        public static DateTime SetUTCToClientTime(this DateTime datetime, string CurrentZone)
        {
            if (!string.IsNullOrWhiteSpace(CurrentZone))
            {
                try
                {

                    string[] time = CurrentZone.Split(":");
                    double hour = 0;
                    double min = 0;
                    if (time.Length == 2 && double.TryParse(time[0], out hour) && double.TryParse(time[1], out min))
                    {
                        datetime = datetime.AddHours(hour);
                        datetime = datetime.AddMinutes(min);
                    }
                    else if (time.Length == 1 && double.TryParse(time[0], out hour))
                    {
                        datetime = datetime.AddHours(hour);
                    }
                }
                catch (Exception e)
                {
                    return datetime;
                }

            }
            return datetime;
        }
        public static DateTime SetClientToUTCTime(this DateTime datetime, string CurrentZone)
        {
            if (!string.IsNullOrWhiteSpace(CurrentZone))
            {
                try
                {

                    string[] time = CurrentZone.Split(":");
                    double hour = 0;
                    double min = 0;
                    if (time.Length == 2 && double.TryParse(time[0], out hour) && double.TryParse(time[1], out min))
                    {
                        datetime = datetime.AddHours(hour * (-1));
                        datetime = datetime.AddMinutes(min * (-1));
                    }
                    else if (time.Length == 1 && double.TryParse(time[0], out hour))
                    {
                        datetime = datetime.AddHours(hour * (-1));
                    }
                }
                catch (Exception e)
                {
                    return datetime;
                }

            }
            return datetime;
        }

        public static DateTime SetDBTodayFromUTCTime(this DateTime datetime)
        {
            try
            {
                if (datetime != new DateTime())
                {
                    datetime = datetime.AddHours(6);
                    datetime = new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0);
                    datetime = datetime.AddHours(-6);
                }
            }
            catch (Exception e)
            {
                return datetime;
            }
            return datetime;
        }
        public static string SQLDateFormat(this string datetime, string Dateformat)
        {
            if (!string.IsNullOrWhiteSpace(datetime))
            {
                char symble = '-';
                int m = 0, d = 0, y = 0;
                if (!string.IsNullOrWhiteSpace(Dateformat))
                {
                    if (Dateformat.Contains('/'))
                    {
                        symble = '/';
                    }
                    else if (Dateformat.Contains(' '))
                    {
                        symble = ' ';
                    }
                    m = Dateformat.ToLower().IndexOf('m');
                    d = Dateformat.ToLower().IndexOf('d');
                    y = Dateformat.ToLower().IndexOf('y');
                }
                string mm = "";
                string dd = "";
                string yyyy = "";
                if (datetime.Contains('T'))
                {
                    datetime = datetime.Split('T')[0];
                    d = 6;
                    m = 4;
                    y = 2;
                }
                if (datetime.Contains('/') && symble != '/')
                {
                    symble = '/';
                    d = 2;
                    m = 0;
                    y = 4;
                }
                if (d < m && m < y)
                {
                    yyyy = datetime.Split(symble)[2];
                    mm = datetime.Split(symble)[1];
                    dd = datetime.Split(symble)[0];
                }
                else if (m < d && d < y)
                {
                    yyyy = datetime.Split(symble)[2];
                    dd = datetime.Split(symble)[1];
                    mm = datetime.Split(symble)[0];
                }
                else if (d > m && m > y)
                {
                    yyyy = datetime.Split(symble)[0];
                    mm = datetime.Split(symble)[1];
                    dd = datetime.Split(symble)[2];
                }
                else
                {
                    yyyy = datetime.Split(symble)[0];
                    mm = datetime.Split(symble)[1];
                    dd = datetime.Split(symble)[2];
                }
                datetime = string.Format("{0}-{1}-{2}", yyyy, mm, dd);
            }

            return datetime;
        }
        public static string SQLDateMinValue(this string datetime)
        {
            if (!string.IsNullOrWhiteSpace(datetime))
            {
                datetime = string.Format("{0}{1}", datetime, " 00:00:00.000");
            }

            return datetime;
        }
        public static string SQLDateMaxValue(this string datetime)
        {
            if (!string.IsNullOrWhiteSpace(datetime))
            {
                datetime = string.Format("{0}{1}", datetime, " 23:59:59.999");
            }

            return datetime;
        }
        public static double TimeZoneToNumber(this string timeZone)
        {
            double result = 0.0;

            if (!string.IsNullOrEmpty(timeZone))
            {
                try
                {
                    result = double.Parse(timeZone);
                }
                catch (Exception e)
                {
                    // Handle exception if necessary
                    Console.WriteLine(e.Message);
                }
            }

            return result;
        }
        public static double GetTimeZoneNumber()
        {
            var CurrentTimeZone = "";
            var TimeZoneNum = 0.0;
            try
            {
                var CookieCurrentUser = _httpContext.Request.Cookies[CookieKeys.MyTime];
                if (CookieCurrentUser != null)
                {
                    CurrentTimeZone = CookieCurrentUser.ToString().Replace(":", ".");
                    TimeZoneNum = TimeZoneToNumber(CurrentTimeZone);

                }
            }
            catch (Exception e)
            {
                return 1.0;
            }


            return TimeZoneNum;
        }

    }


}

using MDUA.Framework;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MDUA.Framework
{
    public static class Extentions
    {
        /// <summary>
        /// Gets NUmber Format from Amount value 
        /// Example ToStringFromAmount(this double value, string format="us")
        /// By Default "us" format other "non-us"
        /// </summary>
        /// 


        private static readonly string globalEncryptionKey = "2V1s8x/AzZxKFb64H4RjLgWmZq4t7w!z";

        public static string ListInttoString(this List<int> ids)
        {
            return string.Join(",", ids);
        }
        public static List<int> StringtoListInt(this string ids)
        {
            return ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                      .Select(s => int.Parse(s.Trim()))
                      .ToList();

        }
        public static string EncryptData(string plainText)
        {
            try
            {
                using Aes aesAlg = Aes.Create();
                aesAlg.Key = Encoding.UTF8.GetBytes(globalEncryptionKey);
                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using MemoryStream msEncrypt = new MemoryStream();
                using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }

                byte[] encrypted = msEncrypt.ToArray();
                var data = Convert.ToBase64String(aesAlg.IV.Concat(encrypted).ToArray());
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string DecryptData(string encryptedText)
        {
            byte[] cipherBytes = Convert.FromBase64String(encryptedText);
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = Convert.FromBase64String(globalEncryptionKey);
            aesAlg.IV = cipherBytes.Take(16).ToArray();

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using MemoryStream msDecrypt = new MemoryStream(cipherBytes, 16, cipherBytes.Length - 16);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }




        public static string ConvertToTitleCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input);
        }



        public static string ToStringFromAmount(this double value, string format = "us")
        {
            if (value == 0)
            {
                return "0.0";
            }
            if (format == "non-us")
                return value.ToString("N", new CultureInfo("hi-IN"));
            else
                return value.ToString("N", new CultureInfo("en-US"));
        }
        public static string ToStringFromAmount(this float value, string format = "us")
        {
            if (value == 0)
            {
                return "0.0";
            }
            if (format == "non-us")
                return value.ToString("N", new CultureInfo("hi-IN"));
            else
                return value.ToString("N", new CultureInfo("en-US"));
        }
        public static string ToStringFromAmount(this int value, string format = "us")
        {
            if (value == 0)
            {
                return "0.0";
            }
            if (format == "non-us")
                return value.ToString("N", new CultureInfo("hi-IN"));
            else
                return value.ToString("N", new CultureInfo("en-US"));
        }
        public static string ToStringFromAmount(this decimal value, string format = "us")
        {
            if (value == 0)
            {
                return "0.0";
            }
            if (format == "non-us")
                return value.ToString("N", new CultureInfo("hi-IN"));
            else
                return value.ToString("N", new CultureInfo("en-US"));
        }
        public static string ToStringDDMMYYYY(this DateTime dateTime)
        {
            return dateTime.ToString("mm-dd-yyyy");
        }
        /// <summary>
        /// Gets the 12:00:00 instance of a DateTime
        /// </summary>
        public static DateTime AbsoluteStart(this DateTime dateTime)
        {
            return dateTime.Date;
        }
        public static string GetDateStringForPrint(string start, string end)
        {
            string datestr = "";
            if (!string.IsNullOrWhiteSpace(start) && !string.IsNullOrWhiteSpace(end))
            {
                string convertedstdate = start;
                start = convertedstdate;
                string convertedenddate = end;
                end = convertedenddate;
                if (start == end)
                {
                    datestr = string.Format("Date: {0}", DateTime.Parse(start).ToString("dd-MM-yyyy"));
                }
                else
                {
                    datestr = string.Format("Date: {0} to {1}", DateTime.Parse(start).ToString("dd-MM-yyyy"), DateTime.Parse(end).ToString("dd-MM-yyyy"));
                }
            }
            else if (!string.IsNullOrWhiteSpace(start) && string.IsNullOrWhiteSpace(end))
            {
                string convertedstdate = start;
                start = convertedstdate;
                datestr = string.Format("Date: {0}", DateTime.Parse(start).ToString("dd-MM-yyyy"));
            }
            else
            {
                datestr = string.Format("Date: {0}", DateTime.UtcNow.AddHours(6).ToString("dd/MM/yyyy"));
            }
            return datestr;
        }
        public static string FormatMinToHM(this string min)
        {
            double mins = 0;
            if (!string.IsNullOrWhiteSpace(min))
            {
                if (min == "H" || min == "A")
                {
                    if (min == "H")
                    {
                        return "-";
                    }
                    return min;
                }
                mins = Convert.ToDouble(min);
            }
            TimeSpan spWorkMin = TimeSpan.FromMinutes(mins);
            string workHours = "00:00";
            if (mins > 0)
            {
                workHours = string.Format("{0:00}:{1:00}", mins / 60, mins % 60);
            }
            return workHours;
        }
        public static string FormatAttnFlag(this string min)
        {
            if (min == "H")
            {
                min = "flag_att_td_h";
            }
            else if (min == "A")
            {
                min = "flag_att_td_a";
            }
            else if (min == "L")
            {
                min = "flag_att_td_l";
            }
            else if (min == "P")
            {
                min = "flag_att_td_p";
            }
            else if (min == "W")
            {
                min = "flag_att_td_w";
            }

            return min;
        }
        public static string FormatAttnFlagText(this string min)
        {
            //if (min == "H")
            //{
            //    min = "-";
            //}
            return min;
        }
        public static string FormatMinToHM(this double mins)
        {
            TimeSpan spWorkMin = TimeSpan.FromMinutes(mins);
            string workHours = "00:00";
            if (mins > -1)
            {
                workHours = string.Format("{0:00}:{1:00}", mins / 60, mins % 60);
            }
            return workHours;
        }
        public static string FormatCurrency(this double money)
        {
            string mon = money.ToString("N2");
            return mon;
        }
        public static string FormatMinToHM(this int mins)
        {
            TimeSpan spWorkMin = TimeSpan.FromMinutes(mins);
            string workHours = "00:00";
            if (mins > 0)
            {
                workHours = string.Format("{0:00}:{1:00}", mins / 60, mins % 60);
            }
            return workHours;
        }
        public static string FormatHourToHM(double one, double two)
        {
            double firstcon = double.Parse(FormatMinToHM(one).Replace(":", "."));
            double seccon = double.Parse(FormatMinToHM(two).Replace(":", "."));
            double diff = double.Parse((firstcon - seccon).toFixed(0));
            string workHours = diff.ToString("N2").Replace(".", ":");
            return workHours;
        }

        /// <summary>
        /// Gets the 11:59:59 instance of a DateTime
        /// </summary>
        public static DateTime AbsoluteEnd(this DateTime dateTime)
        {
            return AbsoluteStart(dateTime).AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// Set zero hour then covert to UTC and final SQL Date format return.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string DateTimeSQLMinTodayDate(this DateTime dateTime)
        {
            return dateTime.SetZeroHour().ToUtc().ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string DateTimeSQLMinYesterDate(this DateTime dateTime)
        {
            return dateTime.SetZeroHour().ToUtc().ToString("yyyy-MM-dd");
        }
        public static string DateTimeSQLMaxYesterDate(this DateTime dateTime)
        {
            return dateTime.SetMaxHour().ToUtc().ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Set max hour then covert to UTC and final SQL Date format return..
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string DateTimeSQLMaxTodayDate(this DateTime dateTime)
        {
            return dateTime.SetMaxHour().ToUtc().ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static bool IsStoreOn(string stat, string ed)
        {
            if (string.IsNullOrWhiteSpace(ed) || string.IsNullOrWhiteSpace(stat))
                return true;

            TimeSpan start = DateTime.Parse(stat).TimeOfDay; //10 o'clock
            TimeSpan end = DateTime.Parse(ed).TimeOfDay; //12 o'clock
            TimeSpan now = DateTime.UtcNow.ToClientTime().TimeOfDay;
            bool isokay = false;
            if ((now >= start) && (now <= end))
            {
                isokay = true;
            }
            return isokay;
        }
        public static double ToCurrencyValue(this double val)
        {
            return Math.Round(val, 2);
        }
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
        #region Date Format
        public static string DateFormat(this DateTime? date)
        {
            if (!date.HasValue || date.Value == new DateTime())
            {
                return "";
            }
            return GetDateFormat(date.Value);
        }
        public static string DateFormat(this DateTime date)
        {
            return GetDateFormat(date);
        }
        public static string DateFormat(this DateTime? date, string format)
        {
            if (!date.HasValue || date.Value == new DateTime())
            {
                return "";
            }
            if (!string.IsNullOrWhiteSpace(format))
            {
                string dateVal = GetDateFormat(date.Value);
                string formatVal = dateVal.Split('/')[2];
                if (format == "day")
                {
                    formatVal = dateVal.Split('/')[1];
                    string suffix = "th";
                    if (int.Parse(formatVal) == 1 || int.Parse(formatVal) % 20 == 1 || int.Parse(formatVal) % 30 == 1)
                    {
                        suffix = "st";
                    }
                    else if (int.Parse(formatVal) == 2 || int.Parse(formatVal) % 20 == 2)
                    {
                        suffix = "nd";
                    }
                    else if (int.Parse(formatVal) == 3 || int.Parse(formatVal) % 20 == 3)
                    {
                        suffix = "rd";
                    }
                    return formatVal + suffix;
                }
                else if (format == "month")
                {
                    formatVal = dateVal.Split('/')[0];
                }
                else if (format == "monthName")
                {
                    formatVal = date.Value.ToString("MMMM");
                }
                return formatVal;
            }
            return GetDateFormat(date.Value);
        }
        public static string DateFormat(this DateTime date, string format)
        {
            if (!string.IsNullOrWhiteSpace(format))
            {
                string dateVal = GetDateFormat(date);
                string formatVal = dateVal.Split('/')[2];
                if (format == "day")
                {
                    formatVal = dateVal.Split('/')[1];
                    string suffix = "th";
                    if (int.Parse(formatVal) == 1 || int.Parse(formatVal) % 20 == 1 || int.Parse(formatVal) % 30 == 1)
                    {
                        suffix = "st";
                    }
                    else if (int.Parse(formatVal) == 2 || int.Parse(formatVal) % 20 == 2)
                    {
                        suffix = "nd";
                    }
                    else if (int.Parse(formatVal) == 3 || int.Parse(formatVal) % 20 == 3)
                    {
                        suffix = "rd";
                    }
                    return formatVal + suffix;
                }
                else if (format == "month")
                {
                    formatVal = dateVal.Split('/')[0];
                }
                else if (format == "monthName")
                {
                    formatVal = date.ToString("MMMM");
                }
                return formatVal;
            }
            return GetDateFormat(date);
        }
        private static string GetDateFormat(DateTime date)
        {
            string FormatDate = "";
            if (date != null && date != new DateTime())
            {
                string monthFormat = "MM";
                string dayFormat = "dd";
                if (date.Day < 10)
                {
                    dayFormat = "d";
                }
                if (date.Month < 10)
                {
                    monthFormat = "M";
                }

                FormatDate = date.ToString(string.Format("{0}/{1}/yy", monthFormat, dayFormat));
            }
            return FormatDate;
        }

        public static string RugDateFormat(DateTime date)
        {
            if (date != null)
            {
                var month = date.ToString("MMM");
                int day = int.Parse(date.ToString("dd"));
                var year_time = date.ToString("yyyy h:mm tt");
                string suffix = "th";
                if (day == 1 || day == 21 || day == 31)
                {
                    suffix = "st";
                }
                else if (day == 2 || day == 22)
                {
                    suffix = "nd";
                }
                else if (day == 3 || day == 23)
                {
                    suffix = "rd";
                }
                return month + " " + day + suffix + ", " + year_time;
                //output: Aug 19th, 2020 11:00 AM
            }
            else
            {
                return "";
            }
        }

        #endregion
        public static T FieldOrDefault<T>(this DataRow row, string columnName)
        {
            return row.IsNull(columnName) ? default(T) : row.Field<T>(columnName);
        }
        public static DataTable ListToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable("WorkSheet");
            foreach (PropertyDescriptor prop in properties)
            {
                if (prop.IsBrowsable)
                {
                    table.Columns.Add(string.IsNullOrWhiteSpace(prop.DisplayName) ? prop.Name : prop.DisplayName, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }
            }
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (prop.IsBrowsable)
                    {
                        row[string.IsNullOrWhiteSpace(prop.DisplayName) ? prop.Name : prop.DisplayName] = prop.GetValue(item) ?? DBNull.Value;
                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }
        public static double MinutesToDecimalHours(this int TotalMunites)
        {
            int Hour = TotalMunites / 60;
            double Munites = TotalMunites % 60;
            Munites = Munites * 0.01666667;
            return Math.Round(Hour + Munites, 2);

        }
        public static double SecondToDecimalHours(this int TotalMunites)
        {
            int Hour = TotalMunites / 3600;
            double Munites = TotalMunites % 3600;
            Munites = Munites * 0.01666667;
            return Math.Round(Hour + Munites, 2);

        }
        public static string ConvertMinuteToHour(this int TotalMunites)
        {
            var timeSpan = TimeSpan.FromMinutes(Convert.ToDouble(TotalMunites));
            double hh = timeSpan.TotalHours > 24 ? timeSpan.TotalHours : timeSpan.Hours;
            double mm = Convert.ToDouble(timeSpan.Minutes);
            double ss = timeSpan.TotalSeconds;
            string m = "";
            if (hh > 0)
            {
                m = hh.toFixed(0) + "h " + mm.toFixed(0) + "m";
            }
            else
            {
                m = mm.toFixed(0) + "min(s)";
            }
            return m;
        }
        public static string toFixed(this double number, uint decimals)
        {
            return number.ToString("N" + decimals);
        }
        #region Phone Number Format
        public static string PhoneNumberFormat(this string Number)
        {
            if (!string.IsNullOrWhiteSpace(Number))
            {
                Number = Number.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");
                if (Number.Length == 10)
                {
                    var sb = new StringBuilder(Number);
                    sb.Insert(3, "-");
                    sb.Insert(7, "-");
                    return sb.ToString();
                }
            }
            return Number;
        }
        public static string DateFormat(this string Number, string format = "")
        {
            if (!string.IsNullOrWhiteSpace(Number))
            {
                if (!string.IsNullOrWhiteSpace(format))
                {
                    if (format.ToLower() == "mm-dd-yyyy")
                    {
                        string[] d = Number.Split("-");
                        string date = d[1] + "-" + d[0] + "-" + d[2];
                        Number = date;
                    }
                }
                else
                {
                    string[] d = Number.Split("-");
                    string date = d[2] + "-" + d[1] + "-" + d[0];
                    Number = date;
                }
            }
            return Number;
        }
        //private static string AppendAtPosition(string baseString, int position, string character)
        //{
        //    var sb = new StringBuilder(baseString);
        //    for (int i = position; i < sb.Length; i += (position + character.Length))
        //        sb.Insert(i, character);
        //    return sb.ToString();
        //}
        #endregion

        public static DateTime ToDateTime(this string StrDateTime)
        {
            /*
             SUPPORTED FORMAT
            //YYYY-MM-DD
            //MM/DD/YYYY
            //MM/DD/YYYY HH:MM:SS TT
            */
            DateTime Date = new DateTime();

            if (!string.IsNullOrWhiteSpace(StrDateTime) && StrDateTime.Split('-').Count() == 3)
            {
                int Year;
                int Month;
                int Day;
                var Datevalues = StrDateTime.Trim().Split('-');
                if (int.TryParse(Datevalues[1], out Month) && Month > 0 && Month < 13
                       && int.TryParse(Datevalues[2], out Day) && Day > 0 && Day < 32
                       && int.TryParse(Datevalues[0], out Year) && Year > 1910)
                {
                    Date = new DateTime(Year, Month, Day);
                }
                return Date;
            }

            if (!string.IsNullOrWhiteSpace(StrDateTime))
            {
                StrDateTime = StrDateTime.Trim();
                string StrDate = "";
                string StrTime = "";
                string AMPM = "";
                bool IsPM = false;

                if (StrDateTime.IndexOf(" ") > 8 && StrDateTime.Split(' ').Count() == 2)
                {
                    StrDate = StrDateTime.Split(' ')[0];
                    StrTime = StrDateTime.Split(' ')[1];
                }
                else if (StrDateTime.IndexOf(" ") > 8 && StrDateTime.Split(' ').Count() == 3)
                {
                    StrDate = StrDateTime.Split(' ')[0];
                    StrTime = StrDateTime.Split(' ')[1];
                    AMPM = StrDateTime.Split(' ')[2];
                    IsPM = AMPM.ToLower() == "pm";
                }
                else
                {
                    StrDate = StrDateTime;
                }

                #region Date
                var Datevalues = StrDate.Trim().Split('/');
                if (Datevalues.Length == 3)
                {
                    int Day = 0;
                    int Month = 0;
                    int Year = 0;
                    if (int.TryParse(Datevalues[0], out Month) && Month > 0 && Month < 13
                        && int.TryParse(Datevalues[1], out Day) && Day > 0 && Day < 32
                        && int.TryParse(Datevalues[2], out Year) && Year > 1910)
                    {
                        Date = new DateTime(Year, Month, Day);
                    }
                }
                #endregion

                #region Time
                if (Date != new DateTime() && !string.IsNullOrWhiteSpace(StrTime))
                {


                    //if(StrTime.IndexOf(" ") > 2)
                    //{
                    //    StrTime = StrTime.Split(' ')[0];
                    //    IsPM = StrTime.Split(' ')[1].ToLower() == "pm";
                    //}

                    var Timevalues = StrTime.Trim().Split(':');
                    #region AddTime
                    if (Timevalues.Length > 1 && Timevalues.Length < 4)
                    {
                        int Hour = 0;
                        int Munite = 0;
                        int Second = 0;

                        if (int.TryParse(Timevalues[0], out Hour) && Hour > -1 && Hour < 24)
                        {
                            if (IsPM && Hour < 13)
                                Hour += 12;

                            Date = Date.AddHours(Hour);
                        }
                        if (int.TryParse(Timevalues[1], out Munite) && Munite > -1 && Munite < 60)
                        {
                            Date = Date.AddMinutes(Munite);
                        }
                        if (Timevalues.Length > 2 && int.TryParse(Timevalues[2], out Second) && Second > -1 && Second < 60)
                        {
                            Date = Date.AddSeconds(Second);
                        }
                    }
                    #endregion
                }
                #endregion
            }
            return Date;
        }

        public static string GetCardType(this string cardNumber)
        {
            cardNumber = cardNumber.Replace("-", "");
            //https://www.regular-expressions.info/creditcard.html
            if (Regex.Match(cardNumber, @"^4[0-9]{12}(?:[0-9]{3})?$").Success)
            {
                return "Visa";
            }

            if (Regex.Match(cardNumber, @"^(?:5[1-5][0-9]{2}|222[1-9]|22[3-9][0-9]|2[3-6][0-9]{2}|27[01][0-9]|2720)[0-9]{12}$").Success)
            {
                return "MasterCard";
            }

            if (Regex.Match(cardNumber, @"^3[47][0-9]{13}$").Success)
            {
                return "AmericanExpress";
            }

            if (Regex.Match(cardNumber, @"^6(?:011|5[0-9]{2})[0-9]{12}$").Success)
            {
                return "Discover";
            }

            if (Regex.Match(cardNumber, @"^(?:2131|1800|35\d{3})\d{11}$").Success)
            {
                return "JCB";
            }
            return "Others";
        }

        public static string ToDateTimeText(this DateTime? datetime)
        {
            if (datetime.HasValue)
            {
                return datetime.Value.ToString("MM/dd/yyyy hh:mm tt");
            }
            else
            {
                return "-";
            }
        }
        public static string ToDateTimeTokenText(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-ddThh:mm:ss");
        }
        public static string ToDateText(this DateTime? datetime)
        {
            if (datetime.HasValue)
            {
                return datetime.Value.ToString("MM/dd/yyyy");
            }
            else
            {
                return "-";
            }
        }
        public static string GenerateBookingNo(this int BookingId)
        {
            string BookingFormat = "00000000";
            BookingFormat = string.Concat(BookingFormat, BookingId);
            BookingFormat = string.Format("BK{0}", BookingFormat.Substring(BookingFormat.Length - 8));
            return BookingFormat;
        }
        public static string GenerateInvoiceNo(this int InvoiceId, string prefix = "")
        {
            if (string.IsNullOrWhiteSpace(prefix))
                prefix = "INV";
            string InvoiceFormat = "00000000";
            InvoiceFormat = string.Concat(InvoiceFormat, InvoiceId);
            InvoiceFormat = string.Format("{1}{0}", InvoiceFormat.Substring(InvoiceFormat.Length - 8), prefix);
            return InvoiceFormat;
        }
        public static string GeneratePosInvoiceNo(this int InvoiceId)
        {
            string InvoiceFormat = "00000000";
            InvoiceFormat = string.Concat(InvoiceFormat, InvoiceId);
            InvoiceFormat = string.Format("POS{0}", InvoiceFormat.Substring(InvoiceFormat.Length - 8));
            return InvoiceFormat;
        }
        public static string GenerateQoutationInvoiceNo(this int InvoiceId)
        {
            string InvoiceFormat = "00000000";
            InvoiceFormat = string.Concat(InvoiceFormat, InvoiceId);
            InvoiceFormat = string.Format("QOUT{0}", InvoiceFormat.Substring(InvoiceFormat.Length - 8));
            return InvoiceFormat;
        }
        public static string GenerateInvoiceBatchNo(this string strDate)
        {
            string InvoiceFormat = "000000000000";
            InvoiceFormat = string.Concat(InvoiceFormat, strDate);
            InvoiceFormat = string.Format("BTH{0}", InvoiceFormat.Substring(InvoiceFormat.Length - 12));
            return InvoiceFormat;
        }
        public static string GenerateEstimateNo(this int EstimateId)
        {
            string InvoiceFormat = "00000000";
            InvoiceFormat = string.Concat(InvoiceFormat, EstimateId);
            InvoiceFormat = string.Format("EST{0}", InvoiceFormat.Substring(InvoiceFormat.Length - 8));
            return InvoiceFormat;
        }
        public static string GeneratePONo(this int EstimateId, string PreText)
        {
            if (string.IsNullOrWhiteSpace(PreText))
            {
                PreText = "PO";
            }
            string InvoiceFormat = "00000000";
            InvoiceFormat = string.Concat(InvoiceFormat, EstimateId);
            InvoiceFormat = string.Format("{1}{0}", InvoiceFormat.Substring(InvoiceFormat.Length - 8), PreText);
            return InvoiceFormat;
        }
        public static string GenerateDONoBranch(this int EstimateId)
        {
            string InvoiceFormat = "00000000";
            InvoiceFormat = string.Concat(InvoiceFormat, EstimateId);
            InvoiceFormat = string.Format("DOB{0}", InvoiceFormat.Substring(InvoiceFormat.Length - 8));
            return InvoiceFormat;
        }
        public static string GenerateDONoTech(this int EstimateId)
        {
            string InvoiceFormat = "00000000";
            InvoiceFormat = string.Concat(InvoiceFormat, EstimateId);
            InvoiceFormat = string.Format("DOT{0}", InvoiceFormat.Substring(InvoiceFormat.Length - 8));
            return InvoiceFormat;
        }
        public static string GenerateBillNO(this int EstimateId)
        {
            string InvoiceFormat = "00000000";
            InvoiceFormat = string.Concat(InvoiceFormat, EstimateId);
            InvoiceFormat = string.Format("BL{0}", InvoiceFormat.Substring(InvoiceFormat.Length - 8));
            return InvoiceFormat;
        }
        public static string ToTimeText(this DateTime? datetime)
        {
            if (datetime.HasValue)
            {
                return datetime.Value.ToString("hh:mm tt");
            }
            else
            {
                return "-";
            }
        }
        public static bool IsValidEmailAddress(this string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static string[] Split(this string data, string SplitBy)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return null;
            }
            return data.Split(new string[] { SplitBy }, StringSplitOptions.None);
        }
        public static string UppercaseFirst(this string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            s = s.ToLower();
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }
        public static string CapitalizeFirst(this string s)
        {
            bool IsNewSentense = true;
            var result = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                if (IsNewSentense && char.IsLetter(s[i]))
                {
                    result.Append(char.ToUpper(s[i]));
                    IsNewSentense = false;
                }
                else
                    result.Append(s[i]);

                if (s[i] == ' ')
                {
                    IsNewSentense = true;
                }
            }

            return result.ToString();
        }
        public static DateTime SetZeroHour(this DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0);
        }
        public static DateTime SetClientZeroHourToUTC(this DateTime datetime, string CookieCurrentUser)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0).ClientToUTCTime(CookieCurrentUser);
        }
        public static DateTime SetMaxHour(this DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day, 23, 59, 59);
        }
        public static DateTime SetClientMaxHourToUTC(this DateTime datetime, string CookieCurrentUser)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day, 23, 59, 59).ClientToUTCTime(CookieCurrentUser);
        }
        public static DateTime UTCCurrentTime(this DateTime datetime)
        {
            datetime = DateTime.UtcNow;
            return datetime;
        }
        public static string ReplaceSpecialChar(this string value, string newvalue = "-")
        {
            if (value == null)
                return "";
            return Regex.Replace(value, @"[^0-9a-zA-Z]+", newvalue);
        }
        public static string ReplaceSpecialQuotation(this string value, string newvalue = "`")
        {
            if (value == null)
                return "";
            return Regex.Replace(value, @"'", newvalue);
        }
        public static string ReplaceSpecialCharFile(this string value, string newvalue = "_")
        {
            if (value == null)
                return "";
            value = value.Replace(" ", "");
            return Regex.Replace(value, @"[^0-9a-zA-Z.]+", newvalue);
        }
        public static string ReplaceSpecialCharForPropertySearch(this string value, string newvalue = "-")
        {
            return Regex.Replace(value, @"[^0-9a-zA-Z()]+", newvalue);
        }
        public static string ReplaceWithMalyasia(this string value)
        {
            return Regex.Replace(value, "Malaysia", "");
        }
        public static string ReplaceSpecialCharWithSingleQuote(this string value, string newvalue = "-")
        {
            return Regex.Replace(value, @"'[^0-9a-zA-Z]+", newvalue);
        }
        public static string ToDateToListString(this DateTime? value)
        {
            return value.HasValue ? "Added On : " + value.Value.ToString("MMM dd") : string.Empty;
        }
        public static List<T> ConvertToList<T>(this DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();

            var properties = typeof(T).GetProperties();

            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();

                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name) && row[pro.Name] != DBNull.Value)
                        pro.SetValue(objT, row[pro.Name]);

                }

                return objT;
            }).ToList();
        }
        public static List<T> GetListModel<T>(this DataTable dt)
        {
            List<T> lst = new List<T>();
            foreach (DataRow dw in dt.Rows)
            {
                Type Tp = typeof(T);
                //create instance of the type
                T obj = Activator.CreateInstance<T>();
                //fetch all properties
                PropertyInfo[] pf = Tp.GetProperties();
                foreach (PropertyInfo pinfo in pf)
                {
                    ///read the implimeted custome atribute for a property
                    object[] colname = pinfo.GetCustomAttributes(typeof(DataTableColName), false);
                    if (colname == null) continue;
                    if (colname.Length == 0) continue;
                    ///read column name from that atribute object
                    string col = (colname[0] as DataTableColName).CoulumnName;
                    if (!dt.Columns.Contains(col)) continue;
                    if (dw[col] == null) continue;
                    if (dw[col] == DBNull.Value) continue;
                    ///set property value
                    pinfo.SetValue(obj, dw[col], null);
                }
                lst.Add(obj);
            }
            return lst;
        }

        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperties().Where(p => p.Name == prop.Name).SingleOrDefault();
                            //obj.GetType().GetProperty(prop.Name);
                            //propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                            if (propertyInfo != null)
                            {
                                Type t = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                                object safeValue = (row[prop.Name] == null) ? null : Convert.ChangeType(row[prop.Name], t);

                                propertyInfo.SetValue(obj, safeValue, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }
        public static List<T> ToListObject<T>(this DataTable? dt) where T : new()
        {
            if (dt is null)
            {
                return [];
            }

            // Create a dictionary to map column names to their ordinal positions.
            var columnIndexMap = dt
                .Columns
                .Cast<DataColumn>()
                .ToDictionary(column => column.ColumnName, column => column.Ordinal);

            // Filter properties of the type T that exist in the DataTable.
            var properties = typeof(T)
                .GetProperties()
                .Where(prop => columnIndexMap.ContainsKey(prop.Name))
                .ToArray();

            // List to store the result.
            var result = new List<T>();

            // Use DataTableReader for efficient reading.
            using var reader = dt.CreateDataReader();
            while (reader.Read())
            {
                var objT = new T();
                foreach (var prop in properties)
                {
                    // Get the column index for the current property.
                    int columnIndex = columnIndexMap[prop.Name];

                    // Check if the value is not DBNull before setting it.
                    if (reader.IsDBNull(columnIndex)) continue;
                    var value = reader.GetValue(columnIndex);
                    prop.SetValue(objT, value);
                }
                result.Add(objT);
            }

            return result;
        }

        public static List<T> ToList<T>(this DataTable datatable) where T : new()
        {
            List<T> Temp = new List<T>();
            try
            {
                List<string> columnsNames = new List<string>();
                foreach (DataColumn DataColumn in datatable.Columns)
                    columnsNames.Add(DataColumn.ColumnName);
                Temp = datatable.AsEnumerable().ToList().ConvertAll<T>(row => ToModel<T>(row, columnsNames));
                return Temp;
            }
            catch { return Temp; }
        }

        public static T ToModel<T>(DataRow row, List<string> columnsName) where T : new()
        {
            T obj = new T();
            try
            {
                string columnname = "";
                string value = "";
                PropertyInfo[] Properties; Properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in Properties)
                {
                    columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
                            {
                                value = row[columnname].ToString().Replace("$", "").Replace(",", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
                            }
                            else
                            {
                                value = row[columnname].ToString().Replace("%", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                            }
                        }
                    }
                }
                return obj;
            }
            catch { return obj; }
        }
        public static float ToFloat(this string value)
        {
            float result = 0;
            float.TryParse(value, out result);
            return result;
        }
        public static bool ToBool(this string value)
        {
            bool result = false;
            bool.TryParse(value, out result);
            return result;
        }
        public static int ToInt(this string value)
        {
            int result = 0;
            int.TryParse(value, out result);
            return result;
        }
        public static decimal ToDecimal(this string value)
        {
            decimal result = 0;
            decimal.TryParse(value, out result);
            return result;
        }
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }



    public class DataTableColName : Attribute
    {
        string coulumnName = string.Empty;
        public string CoulumnName
        {
            get { return coulumnName; }
            set { coulumnName = value; }
        }
        public DataTableColName(string colName)
        {
            coulumnName = colName;
        }
    }

}

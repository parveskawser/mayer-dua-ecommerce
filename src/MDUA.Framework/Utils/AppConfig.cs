using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;
namespace MDUA.Framework.Utils
{
    public static class AppConfig
    {
        private static IHttpContextAccessor HttpContextAccessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }
        public static Uri GetAbsoluteUri()
        {
            var request = HttpContextAccessor.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            uriBuilder.Path = request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();
            return uriBuilder.Uri;
        }
        public static string GetJWTValidAudience(this IConfiguration iconfig)
        {
            string result = string.Empty;
            result = iconfig["JWT:ValidAudience"];
            return result;
        }
        public static string GetJWTValidIssuer(this IConfiguration iconfig)
        {
            string result = string.Empty;
            result = iconfig["JWT:ValidIssuer"];
            return result;
        }
        public static string GetJWTSecret(this IConfiguration iconfig)
        {
            string result = string.Empty;
            result = iconfig["JWT:Secret"];
            return result;
        } 
        private static String ones(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = "";
            switch (_Number)
            {

                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }
            return name;
        }
        private static String tens(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = null;
            switch (_Number)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    name = "Fourty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (_Number > 0)
                    {
                        name = tens(Number.Substring(0, 1) + "0") + " " + ones(Number.Substring(1));
                    }
                    break;
            }
            return name;
        }
        public static byte[] ReadAllBytes(this Stream instream)
        {
            if (instream is MemoryStream)
                return ((MemoryStream)instream).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                instream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        public static String ConvertWholeNumber(String Number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX    
                bool isDone = false;//test if already translated    
                double dblAmt = (Convert.ToDouble(Number));
                //if ((dblAmt > 0) && number.StartsWith("0"))    
                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric    
                    beginsZero = Number.StartsWith("0");

                    int numDigits = Number.Length;
                    int pos = 0;//store digit grouping    
                    String place = "";//digit grouping name:hundres,thousand,etc...    
                    switch (numDigits)
                    {
                        case 1://ones' range    

                            word = ones(Number);
                            isDone = true;
                            break;
                        case 2://tens' range    
                            word = tens(Number);
                            isDone = true;
                            break;
                        case 3://hundreds' range    
                            pos = (numDigits % 3) + 1;
                            place = " Hundred ";
                            break;
                        case 4://thousands' range    
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 7://millions' range    
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " Million ";
                            break;
                        case 10://Billions's range    
                        case 11:
                        case 12:

                            pos = (numDigits % 10) + 1;
                            place = " Billion ";
                            break;
                        //add extra case options for anything above Billion...    
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)    
                        if (Number.Substring(0, pos) != "0" && Number.Substring(pos) != "0")
                        {
                            try
                            {
                                word = ConvertWholeNumber(Number.Substring(0, pos)) + place + ConvertWholeNumber(Number.Substring(pos));
                            }
                            catch { }
                        }
                        else
                        {
                            word = ConvertWholeNumber(Number.Substring(0, pos)) + ConvertWholeNumber(Number.Substring(pos));
                        }

                        //check for trailing zeros    
                        //if (beginsZero) word = " and " + word.Trim();    
                    }
                    //ignore digit grouping names    
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch { }
            return word.Trim();
        }
        public static string GenerateCustomNumber(int length, string head = "")
        {

            const string charsToBeAdded = "1234567890BRINTA";
            var randomNumber = new Random();
            var customNumber = new string(Enumerable.Repeat(charsToBeAdded, length)
                    .Select(s => s[randomNumber.Next(s.Length)])
                    .ToArray());
            var newNumber = head + customNumber;
            return newNumber;
        }
        public static string GetBaseDomain(this IConfiguration iconfig)
        {
            string result = string.Empty;
            result = iconfig["DomainUrl"];
            return result;
        }
        public static string GetImageDomain(this IConfiguration iconfig)
        {
            string result = string.Empty;
            result = iconfig["ImageUrl"];
            return result;
        }
        public static string GetConnectionString(this IConfiguration iconfig)
        {
            string result = string.Empty;
            result = iconfig["ConnectionStrings:ConnectionSCH"];
            return result;
        }


		#region Paze Size

		public static int GetRestaurantMenuCategoryPageSize(this IConfiguration iconfig)
		{
			int result = 0;
			string countstring = iconfig["RestaurantMenuCategoryPageSize"] != null && !string.IsNullOrWhiteSpace(iconfig["RestaurantMenuCategoryPageSize"]) ? iconfig["RestaurantMenuCategoryPageSize"] : "10";
			int.TryParse(countstring, out result);
			if (result == 0)
			{
				result = 10;
			}
			return result;
		}

		public static int GetRestaurantMenuItemPageSize(this IConfiguration iconfig)
		{
			int result = 0;
			string countstring = iconfig["RestaurantMenuItemPageSize"] != null && !string.IsNullOrWhiteSpace(iconfig["RestaurantMenuItemPageSize"]) ? iconfig["RestaurantMenuItemPageSize"] : "10";
			int.TryParse(countstring, out result);
			if (result == 0)
			{
				result = 10;
			}
			return result;
		}

		public static int GetRestaurantSetting_location(this IConfiguration iconfig)
		{
			int result = 0;
			string countstring = iconfig["RestaurantSetting_location"] != null && !string.IsNullOrWhiteSpace(iconfig["RestaurantSetting_location"]) ? iconfig["RestaurantSetting_location"] : "10";
			int.TryParse(countstring, out result);
			if (result == 0)
			{
				result = 10;
			}
			return result;
		}

		public static int GetRestaurantSetting_TableSetup(this IConfiguration iconfig)
		{
			int result = 0;
			string countstring = iconfig["RestaurantSetting_TableSetup"] != null && !string.IsNullOrWhiteSpace(iconfig["RestaurantSetting_TableSetup"]) ? iconfig["RestaurantSetting_TableSetup"] : "10";
			int.TryParse(countstring, out result);
			if (result == 0)
			{
				result = 10;
			}
			return result;
		}

        public static int GetGlobalPageSize(this IConfiguration iconfig)
        {
            int result = 0;
            string countstring = iconfig["GlobalPageSize"] != null && !string.IsNullOrWhiteSpace(iconfig["GlobalPageSize"]) ? iconfig["GlobalPageSize"] : "10";
            int.TryParse(countstring, out result);
            if (result == 0)
            {
                result = 10;
            }
            return result;
        }

        #endregion


        public static string GetDomainVerification(this IConfiguration iconfig)
        {
            string result = string.Empty;
            result = iconfig["DomainVerification"];
            return result;
        }
        public static string GetVersion()
        {
            string result = string.Empty;
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            var root = configurationBuilder.Build();
            result = root.GetSection("Version").Value;
            return result;
        }
        public static string GetConnectionStringLite()
        {
            string _connectionString = string.Empty;
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            var root = configurationBuilder.Build();
            _connectionString = root.GetSection("DevConnectionPPOS").Value;
            return _connectionString;
        }        
        public static string GetConnectionStringCompanyLite()
        {
            string _connectionString = string.Empty;
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            var root = configurationBuilder.Build();
            _connectionString = root.GetSection("ConnectionStrings").GetSection("ConnectionDssMaster").Value;
            return _connectionString;
        }
        public static string GetStaticFileVersion
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }
        public static string GetPageUrl()
        {
            var request = HttpContextAccessor.HttpContext.Request;
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Scheme = request.Scheme;
            uriBuilder.Host = request.Host.Host;
            uriBuilder.Path = request.Path.ToString();
            uriBuilder.Query = request.QueryString.ToString();
            string _Path = uriBuilder.Path;
            return _Path;
        }
        public static string Domain()
        {
            string dom = string.Empty;
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            var root = configurationBuilder.Build();
            dom = root.GetSection("DomainUrl").GetSection("MainUrl").Value;
            return dom;
        }         
        public static string DomUrlWeb()
        {
            string dom = string.Empty;
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            var root = configurationBuilder.Build();
            dom = root.GetSection("DomainUrl").GetSection("DomUrlWeb").Value;
            return dom;
        }
        public static string LoaderLogo()
        {
            string dom = string.Empty;
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            var root = configurationBuilder.Build();
            dom = root.GetSection("LoaderLogo").Value;
            return dom;
        }
        public static string Currency()
        {
            string currency = string.Empty;
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            var root = configurationBuilder.Build();
            currency = root.GetSection("Currency").GetSection("CurrFormat").Value;
            return currency;
        }
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return (request.Headers != null) && (request.Headers["X-Requested-With"] == "XMLHttpRequest");
        }
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                if(item != null)
                {
                    data.Add(item);
                }
            }
            return data;
        }
        public static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}

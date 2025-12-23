using System.Collections;
using System.Diagnostics;

namespace MDUA.Framework
{
    /// <summary>
    /// Summary description for ConfigurationBlock.
    /// </summary>
    public sealed class ConfigurationBlock
    {

        private static Hashtable _hashtable = (Hashtable)AppDomain.CurrentDomain.GetData("Configuration");

        /// <summary>
        /// default constructor for class ConfigurationBlock
        /// </summary>
        public ConfigurationBlock()
        {

        }
        /// <summary>
        /// getting Connection string
        /// </summary>
        /// 
        public static string ConnectionString
        {
            get
            {
                if (System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionSCH"] == null)
                {
                    throw new Exception("Connection string not configured");
                }
                return System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionSCH"].ConnectionString;
                //return _ConnectionString;
            }
        }

        public static string MasterConnectionString
        {
            get
            { 
                // Get call stack
                StackTrace stackTrace = new StackTrace();

                // Get calling method name
                Console.WriteLine(stackTrace.GetFrame(1).GetMethod().Name);
                   
                if (System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionSCH"] == null)
                {
                   throw new Exception("Connection string not configured");
                }
                return System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionSCH"].ConnectionString;
                //return _ConnectionString;
            }
        }

        public static string EntityFrameworkNameSpace
        {
            get
            {
                string _EntityFrameworkNameSpace = string.Empty;
                if (System.Configuration.ConfigurationManager.AppSettings["EntityFrameworkNameSpace"] != null)
                {
                    _EntityFrameworkNameSpace = System.Configuration.ConfigurationManager.AppSettings["EntityFrameworkNameSpace"];
                }
                return _EntityFrameworkNameSpace;
            }
        }

        ///// <summary>
        ///// getting Mail Settings
        ///// </summary>
        //public static MailSettingsSectionGroup MailSettings
        //{
        //    get { return _MailSettings; }
        //}

        /// <summary>
        /// Whether to display trace information or not, from configuration file
        /// </summary>
        public static bool DisplayTraceInformation
        {
            get
            {
                try
                {
                    bool value;
                    string config = "false";

                    if (System.Configuration.ConfigurationManager.AppSettings["DisplayTraceInformation"] != null)
                    {
                        config = System.Configuration.ConfigurationManager.AppSettings["DisplayTraceInformation"].ToString();
                    }

                    if (Boolean.TryParse(config, out value))
                    {
                        return value;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
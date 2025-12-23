using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDUA.Framework.Utils
{
    public abstract class ConfigurationBlock
    {
        /// <summary>
        /// getting Connection string
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                if (System.Configuration.ConfigurationManager.ConnectionStrings["hsCoonectionString"] == null)
                {
                    throw new Exception("Connection string not configured");
                }
                return System.Configuration.ConfigurationManager.ConnectionStrings["hsCoonectionString"].ConnectionString;
                //return _ConnectionString;
            }
        }

        public static bool IsErrorViewable
        {
            get
            {
                bool value = true;
                if (System.Configuration.ConfigurationManager.AppSettings["IsErrorViewable"] != null)
                {
                    Boolean.TryParse(System.Configuration.ConfigurationManager.AppSettings["IsErrorViewable"], out value);
                }
                return value;
            }
        }

        public static int PreviewContractID
        {
            get
            {
                int previewContractID = 21000008;
                if (System.Configuration.ConfigurationManager.AppSettings["PreviewContractID"] != null)
                {
                    Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["PreviewContractID"], out previewContractID);
                }
                return previewContractID;
            }
        }

        public static int PreviewVoucherID
        {
            get
            {
                int previewVoucherID = 12;
                if (System.Configuration.ConfigurationManager.AppSettings["PreviewVoucherID"] != null)
                {
                    Int32.TryParse(System.Configuration.ConfigurationManager.AppSettings["PreviewVoucherID"], out previewVoucherID);
                }
                return previewVoucherID;
            }
        }

        public static string DefaultLanguage
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["DefaultLanguage"];
            }
        }
    
    }
}

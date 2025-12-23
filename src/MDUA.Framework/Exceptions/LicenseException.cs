using System;
using System.Collections.Generic;
using System.Text;

namespace MDUA.Framework.Exceptions
{
    /// <summary>
    /// Class
    /// Name: LicenseException
    /// Description: This class performs as a Custom License Connection Exception for Licensing Layer
    /// 
    /// Last updated on:
    /// September 05, 2009
    /// Change description:    
    /// </summary>
    public class LicenseException : Exception
    {
        public LicenseException():base() { }

        public LicenseException(Exception innerException) : base(String.Empty, innerException) { }
    }
}

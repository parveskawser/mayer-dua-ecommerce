using System;
using System.Collections.Generic;
using System.Text;

namespace MDUA.Framework.Exceptions
{
    /// <summary>
    /// Class
    /// Name: SecurityException
    /// Description: This class performs as a Custom Exception 
    /// for the failure of Security Layer
    /// 
    /// Last updated on:
    /// September 05, 2009
    /// Change description:    
    /// </summary>
    public class SecurityException : Exception
    {
        public SecurityException():base() { }
        public SecurityException(Exception innerException) : base(String.Empty, innerException) { }
        public SecurityException(String message, Exception innerException) : base(String.Empty, innerException) { }
    }
}

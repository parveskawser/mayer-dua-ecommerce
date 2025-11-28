using System;
using System.Collections.Generic;
using System.Text;

namespace MDUA.Framework.Exceptions
{
    /// <summary>
    /// Class
    /// Name: DALException
    /// Description: This class performs as a Custom Exception for Data Access Layer
    /// 
    /// Last updated on:
    /// September 05, 2009
    /// Change description:
    /// </summary>
    public class DALException : Exception
    {
        public DALException():base() { }

        public DALException(Exception innerException) : base(String.Empty, innerException) { }
    }
}

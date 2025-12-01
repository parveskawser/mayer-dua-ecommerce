using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDUA.Framework.Exceptions
{
    /// <summary>
    /// Class
    /// Name: DatabaseConnectionException
    /// Description: This class performs as a Custom Database Connection Exception for Data Access Layer
    /// 
    /// Last updated on:
    /// September 05, 2009
    /// Change description:
    /// </summary>
    public class DatabaseConnectionException : DALException
    {
        public DatabaseConnectionException() : base() { }
        public DatabaseConnectionException(Exception innerException) : base(innerException) { }
    }
}

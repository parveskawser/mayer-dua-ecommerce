using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDUA.Framework.Exceptions
{
    /// <summary>
    /// Class
    /// Name: DataBridgeException
    /// Description: This class performs as a Custom Databridge Exception for managing Databridge Errors
    /// 
    /// Last updated on:
    /// September 17, 2009
    /// Change description:
    /// </summary>
    public class DataBridgeException : Exception
    {
        public DataBridgeException() : base() { }
        public DataBridgeException(string message) : base(message) { }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MDUA.Framework.Exceptions
{
    /// <summary>
    /// Class
    /// Name: ObjectReadOnlyException
    /// Description: This class performs as a Custom Exception for Object to check ReadOnly Permission
    /// 
    /// Last updated on:
    /// September 05, 2009
    /// Change description:    
    /// </summary>
    public class ObjectReadOnlyException : Exception
    {
        public ObjectReadOnlyException():base() { }

        public ObjectReadOnlyException(Exception innerException) : base(String.Empty, innerException) { }
    }
}

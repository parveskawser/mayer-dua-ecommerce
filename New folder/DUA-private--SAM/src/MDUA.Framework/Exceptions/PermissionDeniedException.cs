using System;
using System.Collections.Generic;
using System.Text;

namespace MDUA.Framework.Exceptions
{
    /// <summary>
    /// Class
    /// Name: PermissionDeniedException
    /// Description: This class performs as a Custom SecurityException 
    /// for any type permission denied in Security Layer
    /// 
    /// Last updated on:
    /// September 05, 2009
    /// Change description:    
    /// </summary>
    public class PermissionDeniedException : SecurityException
    {
        public PermissionDeniedException():base() { }

        public PermissionDeniedException(Exception innerException) : base(String.Empty, innerException) { }
    }
}

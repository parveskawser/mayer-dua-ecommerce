using System;
using System.Collections.Generic;
using System.Text;

namespace MDUA.Framework.Exceptions
{
    /// <summary>
    /// Class
    /// Name: ObjectUpdateException
    /// Description: This class performs as a Custom DALException 
    /// for the failure of an update operation in DAL.
    /// 
    /// Last updated on:
    /// September 05, 2009
    /// Change description:    
    /// </summary>
    public class ObjectUpdateException : DALException
    {
        public ObjectUpdateException() : base() { }
        public ObjectUpdateException(BaseBusinessEntity obj, Exception ex) : base(ex) { }
        public ObjectUpdateException(object type, object id, Exception ex) : base(ex) { }
    }
}

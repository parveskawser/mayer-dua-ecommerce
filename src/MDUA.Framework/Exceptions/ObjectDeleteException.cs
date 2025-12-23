using System;
using System.Collections.Generic;
using System.Text;

namespace MDUA.Framework.Exceptions
{
    /// <summary>
    /// Class
    /// Name: ObjectDeleteException
    /// Description: This class performs as a Custom DALException 
    /// for the failure of an delete operation in DAL.
    /// 
    /// Last updated on:
    /// September 05, 2009
    /// Change description:    
    /// </summary>
    public class ObjectDeleteException : DALException
    {
        public ObjectDeleteException() : base() { }
        public ObjectDeleteException(BaseBusinessEntity obj, Exception ex) : base(ex) { }
        public ObjectDeleteException(object type, object id, Exception ex) : base(ex) { }
    }
}

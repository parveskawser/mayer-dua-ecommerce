using System;
using System.Collections.Generic;
using System.Text;

namespace MDUA.Framework.Exceptions
{
    /// <summary>
    /// Class
    /// Name: ObjectInsertException
    /// Description: This class performs as a Custom DALException 
    /// for the failure of an insert operation in DAL.
    /// 
    /// Last updated on:
    /// September 05, 2009
    /// Change description:    
    /// </summary>
    public class ObjectInsertException : DALException
    {
        public ObjectInsertException() : base() { }
        public ObjectInsertException(BaseBusinessEntity obj, Exception ex) : base(ex) { }
        public ObjectInsertException(object type, object id, Exception ex) : base(ex) { }
    }
}

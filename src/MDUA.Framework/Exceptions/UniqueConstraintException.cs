using System;
using System.Collections.Generic;
using System.Text;

namespace MDUA.Framework.Exceptions
{
    /// <summary>
    /// Class
    /// Name: UniqueConstraintException
    /// Description: This class performs as a Custom DALException 
    /// for any unique constraint violation in DAL.
    /// 
    /// Last updated on:
    /// September 05, 2009
    /// Change description:    
    /// </summary>
    public class UniqueConstraintException : DALException
    {
        private BaseBusinessEntity _ExceptionData;

        public BaseBusinessEntity ExceptionData
        {
            get { return _ExceptionData; }
            set { _ExceptionData = value; }
        }

        public UniqueConstraintException(BaseBusinessEntity obj, Exception ex) : base(ex) { ExceptionData = obj; }
        public UniqueConstraintException(object type, object id, Exception ex) : base(ex) { }
    }
}

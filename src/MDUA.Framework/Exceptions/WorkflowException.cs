using System;
using System.Collections.Generic;
using System.Text;

namespace MDUA.Framework.Exceptions
{
    /// <summary>
    /// Class
    /// Name: WorkflowException
    /// Description: This class performs as a Custom Exception 
    /// for the failure in Workflow processes
    /// 
    /// Last updated on:
    /// September 05, 2009
    /// Change description:    
    /// </summary>
    public class WorkflowException : Exception
    {
        public WorkflowException() { }

        public WorkflowException(Exception innerException) : base(String.Empty, innerException) { }
    }
}

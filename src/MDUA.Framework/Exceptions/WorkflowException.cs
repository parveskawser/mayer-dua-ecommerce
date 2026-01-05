using System;

namespace MDUA.Framework.Exceptions
{
    public class WorkflowException : Exception
    {
        public WorkflowException()
        {
        }

        public WorkflowException(string message)
            : base(message)
        {
        }

        public WorkflowException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

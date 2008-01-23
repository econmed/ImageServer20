using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Threading;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Records information about an exception.
    /// </summary>
    public class ExceptionLogEntry : LogEntry
    {
        private string _exceptionClass;
        private string _message;


        /// <summary>
        /// Private no-args constructor to support NHibernate
        /// </summary>
        protected ExceptionLogEntry()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="e"></param>
        /// <param name="details"></param>
        public ExceptionLogEntry(string operation, Exception e, string details)
            :base(operation, details)
        {
            _exceptionClass = e.GetType().FullName;
            _message = e.Message;
        }

        /// <summary>
        /// Gets or sets the name of the exception class.
        /// </summary>
        public string ExceptionClass
        {
            get { return _exceptionClass; }
            set { _exceptionClass = value; }
        }

        /// <summary>
        /// Gets or sets the top-level message exposed by the exception.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
    }
}

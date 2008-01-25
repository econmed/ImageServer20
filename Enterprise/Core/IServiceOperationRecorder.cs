using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Describes the invocation of a service operation, including the arguments, return value, and any exception thrown.
    /// </summary>
    public class ServiceOperationInvocationInfo
    {
        private readonly string _operationName;
        private readonly Type _serviceClass;
        private readonly MethodInfo _operationMethod;
        private readonly object[] _arguments;
        private readonly object _returnValue;
        private readonly Exception _exception;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operationName"></param>
        /// <param name="serviceClass"></param>
        /// <param name="operation"></param>
        /// <param name="args"></param>
        /// <param name="returnValue"></param>
        /// <param name="exception"></param>
        internal ServiceOperationInvocationInfo(string operationName, Type serviceClass, MethodInfo operation, object[] args,
            object returnValue, Exception exception)
        {
            _operationName = operationName;
            _serviceClass = serviceClass;
            _operationMethod = operation;
            _arguments = args;
            _returnValue = returnValue;
            _exception = exception;
        }

        /// <summary>
        /// Gets the logical name of the operation.
        /// </summary>
        public string OperationName
        {
            get { return _operationName; }
        }

        /// <summary>
        /// Gets the class that provides the service implementation.
        /// </summary>
        public Type ServiceClass
        {
            get { return _serviceClass; }
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> object describing the operation.
        /// </summary>
        public MethodInfo OperationMethodInfo
        {
            get { return _operationMethod; }
        }

        /// <summary>
        /// Gets the list of arguments passed to the operation.
        /// </summary>
        public object[] Arguments
        {
            get { return _arguments; }
        }

        /// <summary>
        /// Gets the return value of the operation, or null if an exception was thrown.
        /// </summary>
        public object ReturnValue
        {
            get { return _returnValue; }
        }

        /// <summary>
        /// Gets any unhandled exception thrown from the service operation, or null if the 
        /// operation completed successfully.
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
        }
    }


    /// <summary>
    /// Defines an interface for creating a <see cref="AuditLogEntry"/> that records
    /// information about the invocation of a service operation.
    /// </summary>
    public interface IServiceOperationRecorder
    {
        /// <summary>
        /// Creates a <see cref="AuditLogEntry"/> for the specified service operation invocation,
        /// or returns null if no log entry is created.
        /// </summary>
        /// <remarks>
        /// Return null indicate that no log entry should be created.
        /// </remarks>
        /// <returns>A log entry, or null to indicate that no log entry should be created.</returns>
        AuditLogEntry CreateLogEntry(ServiceOperationInvocationInfo invocationInfo);
    }
}

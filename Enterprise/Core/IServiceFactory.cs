#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Castle.DynamicProxy;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Event data for <see cref="IServiceFactory.ServiceCreation"/>
    /// </summary>
    public class ServiceCreationEventArgs : EventArgs
    {
        private readonly IList<IInterceptor> _interceptors;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interceptors"></param>
        internal ServiceCreationEventArgs(IList<IInterceptor> interceptors)
        {
            _interceptors = interceptors;
        }

        /// <summary>
        /// Gets the list of AOP interceptors that will be applied to the new service instance.
        /// The event handler may modify this list.
        /// </summary>
        public IList<IInterceptor> ServiceOperationInterceptors
        {
            get { return _interceptors; }
        }
    }

    /// <summary>
    /// Defines the interface to a service factory, which instantiates a service based on a specified
    /// contract.
    /// </summary>
    public interface IServiceFactory
    {
        /// <summary>
        /// Occurs when a new service instance is created.
        /// </summary>
        event EventHandler<ServiceCreationEventArgs> ServiceCreation;

        /// <summary>
        /// Obtains an instance of the service that implements the specified contract.
        /// </summary>
        /// <typeparam name="TServiceContract"></typeparam>
        /// <returns></returns>
        TServiceContract GetService<TServiceContract>();

        /// <summary>
        /// Obtains an instance of the service that implements the specified contract.
        /// </summary>
        /// <returns></returns>
        object GetService(Type serviceContract);

        /// <summary>
        /// Lists the service contracts supported by this factory.
        /// </summary>
        /// <returns></returns>
        ICollection<Type> ListServiceContracts();

        /// <summary>
        /// Lists the service classes that provide implementations of the contracts supported by this factory.
        /// </summary>
        /// <returns></returns>
        ICollection<Type> ListServiceClasses();

        /// <summary>
        /// Tests if this factory supports a service with the specified contract.
        /// </summary>
        /// <param name="serviceContract"></param>
        /// <returns></returns>
        bool HasService(Type serviceContract);
    }
}

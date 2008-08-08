#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise
{
    public delegate void ProcedureQueryCallback<T>(T row);

    /// <summary>
    /// Interface used to define stored procedures that that input parameters and return resultant rows.
    /// </summary>
    /// <typeparam name="TInput">Input parameters</typeparam>
    /// <typeparam name="TOutput">The return type</typeparam>
    public interface IProcedureQueryBroker<TInput, TOutput> : IPersistenceBroker
        where TInput : ProcedureParameters
        where TOutput : ServerEntity, new()
    {
        /// <summary>
        /// Retrieves all entities matching the specified criteria.
        /// Caution: this method may return an arbitrarily large result set.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>A list of entities.</returns>
        IList<TOutput> Find(TInput criteria);

        /// <summary>
        /// Retrieves all entities matching the specified criteria,
        /// constrained by the specified page constraint.
        /// </summary>
        /// <param name="criteria">The search criteria.</param>
        /// <param name="callback">The delegate which is supplied query results.</param>
        void Find(TInput criteria, ProcedureQueryCallback<TOutput> callback);

		/// <summary>
		/// Retrieves the first entity matching the specified crtiera.
		/// </summary>
		/// <param name="criteria">The search criteria.</param>
		/// <returns>The entity.</returns>
    	TOutput FindOne(TInput criteria);
    }
}

﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

#pragma warning disable 1591

using System.Xml;
using System.Xml.Schema;

namespace ClearCanvas.Common.Specifications
{
    /// <summary>
    /// Interface for Specification Operators.
    /// </summary>
    public interface IXmlSpecificationCompilerOperator
    {
        /// <summary>
        /// The XML Tag for the operator.
        /// </summary>
        string OperatorTag { get; }
        /// <summary>
        /// Compile the operator.
        /// </summary>
        /// <param name="xmlNode">The XML Node associated with the operator.</param>
        /// <param name="context">A context for the compiler.</param>
        /// <returns>A compiled <see cref="Specification"/>.</returns>
        Specification Compile(XmlElement xmlNode, IXmlSpecificationCompilerContext context);
        /// <summary>
        /// Get an XmlSchema element that describes the schema for the operator element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is assumed that a simple <see cref="XmlSchemaElement"/> is returned for the 
        /// operator.  The compiler combine the elements for each operator together into an
        /// <see cref="XmlSchema"/>.  If the specific element allows subelements, it should 
        /// be declared to allow any elements from the local namespace/Schema.
        /// </para>
        /// </remarks>
        /// <returns>The Schema element.</returns>
        XmlSchemaElement GetSchema();
    }
}

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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Common;
using System.Reflection;
using NHibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;
using System.Data;

namespace ClearCanvas.Enterprise.Hibernate
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class UniqueConstraintValidationBroker : Broker, IUniqueConstraintValidationBroker
    {
        #region IUniqueConstraintValidationBroker Members

        /// <summary>
        /// Tests whether the specified object satisfies the specified unique constraint.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <param name="uniqueConstraintMembers">The properties of the object that form the unique key.
        /// These may be compound property expressions (e.g. Name.FirstName, Name.LastName).
        /// </param>
        /// <returns></returns>
        public bool IsUnique(DomainObject obj, string[] uniqueConstraintMembers)
        {
            Platform.CheckForNullReference(obj, "obj");
            Platform.CheckForNullReference(uniqueConstraintMembers, "uniqueConstraintMembers");

            if (uniqueConstraintMembers.Length == 0)
                throw new InvalidOperationException("uniqueConstraintMembers must contain at least one entry.");

            HqlQuery hqlQuery = BuildQuery(obj, uniqueConstraintMembers);

            // create a new session to do the validation query
            // this is a bit of a HACK, but we know that this may occur during an interceptor callback
            // on the originating session, and we don't want to modify the state of that session
            // ideally the new session should share the connection and transaction of the main session,
            // but this isn't possible with NHibernate 1.2.0
            using (ISession auxSession = this.Context.PersistentStore.SessionFactory.OpenSession())
            {
                // since we are forced to use a separate transaction, use ReadUncommitted isolation level
                // we want to avoid deadlocking here at all costs, even if it means dirty reads
                // the worst the can happen with a dirty read is that validation will be incorrect,
                // but that's not the end of the world, because the DB will still enforce uniqueness
                ITransaction tx = auxSession.BeginTransaction(IsolationLevel.ReadUncommitted);
                auxSession.FlushMode = FlushMode.Never;
                IQuery query = auxSession.CreateQuery(hqlQuery.Hql);
                int i = 0;
                foreach (HqlCondition condition in hqlQuery.Conditions)
                {
                    foreach(object paramVal in condition.Parameters)
                        query.SetParameter(i++, paramVal);
                }

                long count = query.UniqueResult<long>();

                tx.Commit();    // no data will be written (Flushmode.Never, and we didn't write anything anyways)

                // if count == 0, there are no other objects with this particular set of values
                return count == 0;
            }
        }

        #endregion

        private HqlQuery BuildQuery(DomainObject obj, string[] uniqueConstraintMembers)
        {
            // get the id of the object being validated
            // if the object is unsaved, this id may be null
            object id = (obj is Entity) ? (obj as Entity).OID : (obj as EnumValue).Code;

            HqlQuery query = new HqlQuery(string.Format("select count(*) from {0} x", obj.GetType().Name));
            if (id != null)
            {
                // this object may have already been saved (i.e. this is an update) and therefore should be
                // excluded
                query.Conditions.Add(new HqlCondition("x.id <> ?", new object[] { id }));
            }

            // add other conditions 
            foreach (string propertyExpression in uniqueConstraintMembers)
            {
                object value = EvaluatePropertyExpression(obj, propertyExpression);
                if (value == null)
                    query.Conditions.Add(new HqlCondition(string.Format("x.{0} is null", propertyExpression), new object[] { }));
                else
                    query.Conditions.Add(new HqlCondition(string.Format("x.{0} = ?", propertyExpression), new object[] { value }));
            }

            return query;
       }

        private object EvaluatePropertyExpression(object subject, string propertyExpression)
        {
            if (string.IsNullOrEmpty(propertyExpression))
                throw new InvalidOperationException("propertyExpression must be non-empty.");

            string[] parts = propertyExpression.Split('.');
            return EvaluatePropertyExpression(subject, parts, 0);
        }

        private object EvaluatePropertyExpression(object subject, string[] parts, int partIndex)
        {
            // find property
            Type subjectType = subject.GetType();
            PropertyInfo propertyInfo = subjectType.GetProperty(parts[partIndex]);
            if(propertyInfo == null)
                throw new InvalidOperationException(string.Format("{0} is not a property of type {1}.",
                    parts[partIndex], subjectType.FullName));

            // find get method
            MethodInfo getter = propertyInfo.GetGetMethod(true);
            if (getter == null)
                throw new InvalidOperationException(string.Format("Property {0} of type {1} has no accessible get method.",
                    propertyInfo.Name, subjectType.FullName));

            // evaluate 
            object result = getter.Invoke(subject, null);

            // if null, can't go any further with the expression
            if (result == null)
                return null;

            // return the result if this is the end of the expression, otherwise recur
            return (++partIndex == parts.Length) ? result : EvaluatePropertyExpression(result, parts, partIndex);
        }

    }
}

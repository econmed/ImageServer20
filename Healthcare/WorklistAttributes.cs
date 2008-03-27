using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Abstract base class for worklist attribute classes.
    /// </summary>
    public abstract class WorklistAttribute : Attribute
    {
        
    }

    /// <summary>
    /// When applied to a subclass of <see cref="Worklist"/>, declares the subclass of <see cref="ProcedureTypeGroup"/>
    /// that the worklist is based on.
    /// </summary>
    /// <remarks>
    /// This attribute is inherited.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WorklistProcedureTypeGroupClassAttribute : WorklistAttribute
    {
        private readonly Type _procedureTypeGroupClass;

        public WorklistProcedureTypeGroupClassAttribute(Type procedureTypeGroupClass)
        {
            _procedureTypeGroupClass = procedureTypeGroupClass;
        }

        public Type ProcedureTypeGroupClass
        {
            get { return _procedureTypeGroupClass; }    
        }
    }

    /// <summary>
    /// When applied to a subclass of <see cref="Worklist"/>, indicates whether the class supports time-window filters.
    /// If this attribute is not applied, it is assumed that the class <b>does</b> support time filtering.
    /// </summary>
    /// <remarks>
    /// This attribute is inherited.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class WorklistSupportsTimeFilterAttribute : WorklistAttribute
    {
        private readonly bool _supportsTimeFilter;

        public WorklistSupportsTimeFilterAttribute(bool supportsTimeFilter)
        {
            _supportsTimeFilter = supportsTimeFilter;
        }

        public bool SupportsTimeFilter
        {
            get { return _supportsTimeFilter; }
        }
    }
}

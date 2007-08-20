using System.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common;
using System;

namespace ClearCanvas.Healthcare
{
    [ExtensionPoint]
    public class WorklistExtensionPoint : ExtensionPoint<IWorklist>
    { }

    public interface IWorklistItemKey
    {
    }

    public interface IWorklistItem
    {
        IWorklistItemKey Key { get; set; }
    }

    public interface IWorklist
    {
        IList GetWorklist(Staff currentUserStaff, IPersistenceContext context);
        int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context);
        string DisplayName { get; }
    }
}
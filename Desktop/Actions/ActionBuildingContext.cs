using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Default implementation of <see cref="IActionBuildingContext"/>.
    /// </summary>
    internal class ActionBuildingContext : IActionBuildingContext
    {
        private readonly string _actionID;
        private readonly object _actionTarget;
        private readonly ResourceResolver _resolver;
        private Action _action;

        internal ActionBuildingContext(string actionID, object actionTarget)
        {
            _actionID = actionID;
            _actionTarget = actionTarget;

            _resolver = new ActionResourceResolver(_actionTarget);
        }

        public string ActionID
        {
            get { return _actionID; }
        }

        public Action Action
        {
            get { return _action; }
            set { _action = value; }
        }

        public IResourceResolver ResourceResolver
        {
            get { return _resolver; }
        }

        public object ActionTarget
        {
            get { return _actionTarget; }
        }
    }
}

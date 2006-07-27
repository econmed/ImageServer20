using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Models a toolbar button action, and acts as a binding between the user-interface and a tool.
    /// </summary>
    public class ButtonAction : ClickAction
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="actionID"></param>
        /// <param name="path"></param>
        /// <param name="target"></param>
        /// <param name="flags"></param>
        public ButtonAction(string actionID, Path path, object target, ClickActionFlags flags)
            : base(actionID, ActionCategory.ToolbarAction, path, target, flags)
        {
        }
    }
}

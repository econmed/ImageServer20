using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// Declares an observer binding for the visible state of an action.
	/// </summary>
	/// <remarks>
	/// This attribute causes the visible state of the action specified by the action ID to be
	/// bound to the state of the specified property on the class to which this attribute applies.
	/// The property name must refer to a public boolean property on the target class that has get access.
	/// The change event name must refer to a public event on the class that will fire whenever the state of the property
	/// changes.
	/// </remarks>

	public class VisibleStateObserverAttribute : StateObserverAttribute
	{
		/// <summary>
		/// Attribute constructor
		/// </summary>
		/// <param name="actionID">The logical action identifier to which this attribute applies</param>
		/// <param name="propertyName">The name of the property to bind to</param>
		/// <param name="changeEventName">The name of the property change notification event to bind to</param>
		public VisibleStateObserverAttribute(string actionID, string propertyName, string changeEventName)
			: base(actionID, propertyName, changeEventName)
		{
		}

		internal override void Apply(IActionBuildingContext builder)
		{
            Bind<bool>(builder, "Visible", "VisibleChanged");
        }
	}

}

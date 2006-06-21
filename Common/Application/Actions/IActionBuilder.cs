using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Actions
{
    /// <summary>
    /// Defines the interface for an action builder.  An action builder is an object that
    /// knows how to construct an action from a set of action attributes.
    /// </summary>
    internal interface IActionBuilder
    {
        void Apply(ButtonActionAttribute a);
        void Apply(ClickHandlerAttribute a);
        void Apply(IconSetAttribute a);
        void Apply(MenuActionAttribute a);
        void Apply(TooltipAttribute a);

        void Apply(CheckedStateObserverAttribute a);
        void Apply(EnabledStateObserverAttribute a);
    }
}

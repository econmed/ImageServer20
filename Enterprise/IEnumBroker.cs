using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Defines the interface for a broker that returns instances of <see cref="EnumTable"/> to the application.
    /// </summary>
    public interface IEnumBroker<e, E> : IPersistenceBroker
        where e : struct
        where E : EnumValue<e>
    {
        /// <summary>
        /// Loads the domain enumeration corresponding to the specified C# enum type.
        /// </summary>
        /// <typeparam name="TEnum">The C# enum type on which the enumeration is based.</typeparam>
        /// <returns>a domain enumeration table</returns>
        EnumTable<e, E> Load();
    }
}

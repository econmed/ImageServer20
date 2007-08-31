using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model.Brokers
{
    /// <summary>
    /// Broker for loading <see cref="StatusEnum"/> values.
    /// </summary>
    public interface IStatusEnum : IEnumBroker<StatusEnum>
    {
    }
}

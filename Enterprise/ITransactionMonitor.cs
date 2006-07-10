using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Specifies the inteface to the transaction monitor.  This is very preliminary and will likely
    /// change.
    /// </summary>
    public interface ITransactionMonitor
    {
        /// <summary>
        /// Allows a client to subscribe for change notifications for a given type of entity.
        /// It is extremely important that clients explicitly unsubscribe in order that resources
        /// are properly released.
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="eventHandler"></param>
        void Subscribe(Type entityType, EventHandler<EntityChangeEventArgs> eventHandler);

        /// <summary>
        /// Allows a client to unsubscribe from change notifications for a given type of entity.
        /// It is extremely important that clients explicitly unsubscribe in order that resources
        /// are properly released.
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="eventHandler"></param>
        void Unsubscribe(Type entityType, EventHandler<EntityChangeEventArgs> eventHandler);

        /// <summary>
        /// Allows a client to queue a set of changes for notification to other clients.
        /// Notifications will be sent when <see cref="PostPending"/> is called.
        /// </summary>
        /// <param name="changeSet"></param>
        void Queue(EntityChange[] changeSet);

        /// <summary>
        /// Flushes the queue of change notifications, notifying any subscribers.
        /// </summary>
        void PostPending();
    }
}

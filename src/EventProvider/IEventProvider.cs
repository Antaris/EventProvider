namespace Antaris.EventProvider
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the required contract for implementing an event aggregator.
    /// </summary>
    public interface IEventProvider
    {
        /// <summary>
        /// Creates an event subscription for the given payload type.
        /// </summary>
        /// <typeparam name="TPayload">The payload type.</typeparam>
        /// <param name="token">The subscription token.</param>
        /// <param name="notificationFunc">The notification function used to publish new data against the subscription.</param>
        /// <param name="filterFunc">[Optional] The filter function used to filter payload data before passing to the subscriber.</param>
        /// <returns>The event subscription.</returns>
        IEventSubscription<TPayload> CreateEventSubscription<TPayload>(SubscriptionToken token, Func<TPayload, CancellationToken, Task> notificationFunc, Func<TPayload, CancellationToken, Task<bool>> filterFunc = null);

        /// <summary>
        /// Gets the event of the given type.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <returns>The event instance.</returns>
        TEvent GetEvent<TEvent>() where TEvent : IEvent;

        /// <summary>
        /// Gets the event of the given type.
        /// </summary>
        /// <typeparam name="T">The event type.</typeparam>
        /// <param name="factory">The factory delegate used to create an instance of the event.</param>
        /// <returns>The event instance.</returns>
        TEvent GetEvent<TEvent>(Func<TEvent> factory) where TEvent : IEvent;

        /// <summary>
        /// Gets any external event subscribers provided through the DI system.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <typeparam name="TPayload">The payload type.</typeparam>
        /// <returns>The set of external event subscribers.</returns>
        IEnumerable<IEventSubscriber<TEvent, TPayload>> GetExternalEventSubscribers<TEvent, TPayload>() where TEvent : IEvent;
    }
}
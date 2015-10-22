namespace Antaris.EventProvider
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the required contract for implementing an event subscription.
    /// </summary>
    public interface IEventSubscription<TPayload>
    {
        /// <summary>
        /// Gets the filter delegate.
        /// </summary>
        Func<TPayload, CancellationToken, Task<bool>> FilterFunc { get; }

        /// <summary>
        /// Gets whether the subscription is alive.
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Gets the notification delegate.
        /// </summary>
        Func<TPayload, CancellationToken, Task> NotificationFunc { get; }

        /// <summary>
        /// Gets the subscription token.
        /// </summary>
        SubscriptionToken Token { get; }
    }
}
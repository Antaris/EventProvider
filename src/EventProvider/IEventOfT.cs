namespace Antaris.EventProvider
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the required contract for implementing an event that supports payload data.
    /// </summary>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    public interface IEvent<TPayload> : IEvent
    {
        /// <summary>
        /// Publishes the payload data against any registered subscribers.
        /// </summary>
        /// <param name="payload">The payload data.</param>
        /// <param name="cancellationToken">[Optional] The cancellation token used to cancel asynchronous operations.</param>
        /// <returns>An instance of <see cref="Task"/>.</returns>
        Task PublishAsync(TPayload payload, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Subscribes to the current event using the given notification function.
        /// </summary>
        /// <param name="notificationFunc">The notification function.</param>
        /// <param name="filterFunc">[Optional] The filter function used to filter payload data before notifying attached subscribers.</param>
        /// <returns>The subscription token of the new subscription.</returns>
        SubscriptionToken Subscribe(Func<TPayload, CancellationToken, Task> notificationFunc, Func<TPayload, CancellationToken, Task<bool>> filterFunc = null);

        /// <summary>
        /// Unsubscribes from the event.
        /// </summary>
        /// <param name="token">The subscription token.</param>
        void Unsubscribe(SubscriptionToken token);
    }
}
namespace Antaris.EventProvider
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the required contract for implementing an event subscriber.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    public interface IEventSubscriber<TEvent, TPayload> where TEvent : IEvent
    {
        /// <summary>
        /// Filters the payload data.
        /// </summary>
        /// <param name="payload">The payload data.</param>
        /// <param name="cancellationToken">[Optional] The cancellation token used to cancel asynchronous operations.</param>
        /// <returns>An instance of <see cref="Task"/>.</returns>
        Task<bool> FilterAsync(TPayload payload, CancellationToken cancelationToken = default(CancellationToken));

        /// <summary>
        /// Notifies this subscriber of new payload data.
        /// </summary>
        /// <param name="payload">The payload data.</param>
        /// <param name="cancellationToken">[Optional] The cancellation token used to cancel asynchronous operations.</param>
        /// <returns>An instance of <see cref="Task"/>.</returns>
        Task NotifyAsync(TPayload payload, CancellationToken cancellationToken = default(CancellationToken));
    }
}
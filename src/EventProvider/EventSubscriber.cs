namespace Antaris.EventProvider
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a base implementation of an event subscriber.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    public abstract class EventSubscriber<TEvent, TPayload> : IEventSubscriber<TEvent, TPayload> where TEvent : IEvent
    {
        /// <inheritdoc />
        public virtual Task<bool> FilterAsync(TPayload payload, CancellationToken cancelationToken = default(CancellationToken))
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public abstract Task NotifyAsync(TPayload payload, CancellationToken cancellationToken = default(CancellationToken));
    }
}
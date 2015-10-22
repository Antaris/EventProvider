namespace Antaris.EventProvider
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a subscription to an event.
    /// </summary>
    public class EventSubscription<TPayload> : IEventSubscription<TPayload>
    {
        private readonly WeakReference<Func<TPayload, CancellationToken, Task>> _notificationReference;
        private readonly WeakReference<Func<TPayload, CancellationToken, Task<bool>>> _filterReference;

        /// <summary>
        /// Initialises a new instance of <see cref="EventSubscription{TPayload}"/>.
        /// </summary>
        /// <param name="token">The subscription token.</param>
        /// <param name="notificationFunc">The notification function.</param>
        /// <param name="filterFunc">[Optional] The filter function.</param>
        public EventSubscription(SubscriptionToken token, Func<TPayload, CancellationToken, Task> notificationFunc, Func<TPayload, CancellationToken, Task<bool>> filterFunc = null)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (notificationFunc == null)
            {
                throw new ArgumentNullException(nameof(notificationFunc));
            }

            Token = token;
            _notificationReference = new WeakReference<Func<TPayload, CancellationToken, Task>>(notificationFunc);

            filterFunc = filterFunc ?? ((p, ct) => Task.FromResult(true));
            _filterReference = new WeakReference<Func<TPayload, CancellationToken, Task<bool>>>(filterFunc);
        }

        /// <inheritdoc />
        public Func<TPayload, CancellationToken, Task<bool>> FilterFunc
        {
            get
            {
                return IsAlive ? _filterReference.Target : null;
            }
        }

        /// <inheritdoc />
        public bool IsAlive { get { return _notificationReference.IsAlive; } }

        /// <inheritdoc />
        public Func<TPayload, CancellationToken, Task> NotificationFunc
        {
            get
            {
                return IsAlive ? _notificationReference.Target : null;
            }
        }

        /// <inheritdoc />
        public SubscriptionToken Token { get; private set; }
    }
}
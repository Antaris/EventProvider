namespace Antaris.EventProvider
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a base implementation of an event.
    /// </summary>
    /// <typeparam name="TPayload">The payload type.</typeparam>
    public abstract class Event<TPayload> : IEvent<TPayload>
    {
        private readonly ConcurrentDictionary<SubscriptionToken, IEventSubscription<TPayload>> _subscriptions = new ConcurrentDictionary<SubscriptionToken, IEventSubscription<TPayload>>();
        private static readonly Type _baseType = typeof(Event<TPayload>);
        private static readonly Type _payloadType = typeof(TPayload);
        private static readonly MethodInfo _getExternalSubscribersMethod = _baseType.GetMethod(nameof(GetExternalSubscribers));
        private readonly Lazy<Delegate> _getExternalSubscribersDelegate;
        private readonly Type _eventType;
        private bool? _hasExternalSubscribers;

        /// <summary>
        /// Initialises a new instance of <see cref="Event{TPayload}"/>
        /// </summary>
        protected Event()
        {
            _eventType = GetType();
            _getExternalSubscribersDelegate = new Lazy<Delegate>(() => DelegateCache.GetDelegate(_getExternalSubscribersMethod, _eventType, _payloadType));
        }

        /// <inheritdoc />
        public IEventProvider EventProvider { get; set; }

        /// <inheritdoc />
        public async Task PublishAsync(TPayload payload, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_subscriptions.Count > 0)
            {
                var subscriptions = _subscriptions.Values.ToList();
                foreach (var subscription in subscriptions)
                {
                    if (!subscription.IsAlive)
                    {
                        Unsubscribe(subscription.Token);

                        continue;
                    }

                    if (await subscription.FilterFunc(payload, cancellationToken))
                    {
                        await subscription.NotificationFunc(payload, cancellationToken);
                    }
                }
            }

            IEnumerable<Tuple<Func<TPayload, CancellationToken, Task>, Func<TPayload, CancellationToken, Task<bool>>>> externals = null;

            if (_hasExternalSubscribers == null)
            {
                externals = (IEnumerable<Tuple<Func<TPayload, CancellationToken, Task>, Func<TPayload, CancellationToken, Task<bool>>>>)_getExternalSubscribersDelegate.Value.DynamicInvoke(this);

                _hasExternalSubscribers = externals.Any();
            }

            if (_hasExternalSubscribers.Value && externals == null)
            {
                externals = (IEnumerable<Tuple<Func<TPayload, CancellationToken, Task>, Func<TPayload, CancellationToken, Task<bool>>>>)_getExternalSubscribersDelegate.Value.DynamicInvoke(this);
            }

            foreach (var external in externals)
            {
                if (await external.Item2(payload, cancellationToken))
                {
                    await external.Item1(payload, cancellationToken);
                }
            }
        }

        /// <inheritdoc />
        public SubscriptionToken Subscribe(Func<TPayload, CancellationToken, Task> notificationFunc, Func<TPayload, CancellationToken, Task<bool>> filterFunc = null)
        {
            var token = new SubscriptionToken(st => Unsubscribe(st));
            var subscription = EventProvider.CreateEventSubscription<TPayload>(token, notificationFunc, filterFunc);

            _subscriptions.TryAdd(token, subscription);

            return token;
        }

        /// <inheritdoc />
        public void Unsubscribe(SubscriptionToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            IEventSubscription<TPayload> subscription;
            _subscriptions.TryRemove(token, out subscription);
        }

        /// <summary>
        /// Gets the set of external subscribers provided through the DI system.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <returns>The set of event subscription delegates as ordered pairs.</returns>
        public IEnumerable<Tuple<Func<TPayload, CancellationToken, Task>, Func<TPayload, CancellationToken, Task<bool>>>> GetExternalSubscribers<TEvent>() where TEvent : IEvent
        {
            if (EventProvider == null)
            {
                return Enumerable.Empty<Tuple<Func<TPayload, CancellationToken, Task>, Func<TPayload, CancellationToken, Task<bool>>>>();
            }

            return EventProvider
                .GetExternalEventSubscribers<TEvent, TPayload>()
                .Select(es => Tuple.Create<Func<TPayload, CancellationToken, Task>, Func<TPayload, CancellationToken, Task<bool>>>(
                    (p, ct) => es.NotifyAsync(p, ct),
                    (p, ct) => es.FilterAsync(p, ct)
                )
            );
        }
    }
}
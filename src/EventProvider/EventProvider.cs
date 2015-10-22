namespace Antaris.EventProvider
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Framework.DependencyInjection;

    /// <summary>
    /// Provides event aggregation services.
    /// </summary>
    public class EventProvider : IEventProvider
    {
        private readonly ConcurrentDictionary<Type, IEvent> _events = new ConcurrentDictionary<Type, IEvent>();
        private Func<IServiceProvider> _serviceProviderAccessor;

        /// <summary>
        /// Initialises a new instance of <see cref="EventAggregator"/>
        /// </summary>
        /// <param name="serviceProviderAccessor">The initial service provider accessor.</param>
        public EventProvider(Func<IServiceProvider> serviceProviderAccessor)
        {
            SetServiceProviderAccessor(serviceProviderAccessor);
        }

        /// <inheritdoc />
        public IEventSubscription<TPayload> CreateEventSubscription<TPayload>(SubscriptionToken token, Func<TPayload, CancellationToken, Task> notificationFunc, Func<TPayload, CancellationToken, Task<bool>> filterFunc = null)
        {
            return new EventSubscription<TPayload>(token, notificationFunc, filterFunc);
        }

        /// <inheritdoc />
        public TEvent GetEvent<TEvent>() where TEvent : IEvent
        {
            return GetEventInternal<TEvent>();
        }

        /// <inheritdoc />
        public TEvent GetEvent<TEvent>(Func<TEvent> factory) where TEvent : IEvent
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return GetEventInternal<TEvent>();
        }

        /// <inheritdoc />
        public IEnumerable<IEventSubscriber<TEvent, TPayload>> GetExternalEventSubscribers<TEvent, TPayload>() where TEvent : IEvent
        {
            var serviceProvider = _serviceProviderAccessor();

            if (serviceProvider == null)
            {
                return Enumerable.Empty<IEventSubscriber<TEvent, TPayload>>();
            }

            return serviceProvider.GetService<IEnumerable<IEventSubscriber<TEvent, TPayload>>>();
        }

        /// <inheritdoc />
        public void SetServiceProviderAccessor(Func<IServiceProvider> serviceProviderAccessor)
        {
            if (serviceProviderAccessor == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderAccessor));
            }

            _serviceProviderAccessor = serviceProviderAccessor;
        }

        /// <summary>
        /// Gets the event of the given type.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="factory">[Optional] The factory function used to create the event.</param>
        /// <returns>The event instance.</returns>
        private TEvent GetEventInternal<TEvent>(Func<TEvent> factory = null) where TEvent : IEvent
        {
            var type = typeof(TEvent);

            return (TEvent)_events.GetOrAdd(type, (t) =>
            {
                if (factory == null)
                {
                    var serviceProvider = _serviceProviderAccessor();
                    factory = (serviceProvider != null)
                        ? (Func<TEvent>)(() => (TEvent)ActivatorUtilities.CreateInstance(serviceProvider, type))
                        : (() => (TEvent)Activator.CreateInstance(type));
                }

                var ev = factory();
                ev.EventProvider = this;

                return ev;
            });
        }
    }
}
namespace Antaris.EventProvider
{
    using Microsoft.Framework.DependencyInjection;

    /// <summary>
    /// Provides extensions for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a scoped event subscriber to the services collection.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <typeparam name="TPayload">The payload type.</typeparam>
        /// <typeparam name="TSubscriber">The subscriber implementation type.</typeparam>
        /// <param name="services">The set of services.</param>
        /// <returns>The set of services.</returns>
        public static IServiceCollection AddScopedEventSubscriber<TEvent, TPayload, TSubscriber>(this IServiceCollection services)
            where TEvent : IEvent<TPayload>
            where TSubscriber : class, IEventSubscriber<TEvent, TPayload>
        {
            return services.AddScoped<IEventSubscriber<TEvent, TPayload>, TSubscriber>();
        }

        /// <summary>
        /// Adds a transient event subscriber to the services collection.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <typeparam name="TPayload">The payload type.</typeparam>
        /// <typeparam name="TSubscriber">The subscriber implementation type.</typeparam>
        /// <param name="services">The set of services.</param>
        /// <returns>The set of services.</returns>
        public static IServiceCollection AddTransientEventSubscriber<TEvent, TPayload, TSubscriber>(this IServiceCollection services) 
            where TEvent : IEvent<TPayload>
            where TSubscriber : class, IEventSubscriber<TEvent, TPayload>
        {
            return services.AddTransient<IEventSubscriber<TEvent, TPayload>, TSubscriber>();
        }
    }
}
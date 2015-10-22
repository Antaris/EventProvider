namespace Antaris.EventProvider
{
    using System;

    /// <summary>
    /// Represents a token used to unsubscribe from an event.
    /// </summary>
    public class SubscriptionToken : IDisposable
    {
        private readonly Action<SubscriptionToken> _unsubscribeAction;

        /// <summary>
        /// Initialises a new instance of <see cref="SubscriptionToken"/>.
        /// </summary>
        /// <param name="unsubscribeAction">The unsubscibe action.</param>
        public SubscriptionToken(Action<SubscriptionToken> unsubscribeAction)
        {
            if (unsubscribeAction == null)
            {
                throw new ArgumentNullException(nameof(unsubscribeAction));
            }
        }

        /// <summary>
        /// Gets the unique identifier for this token.
        /// </summary>
        public Guid Token { get; private set; } = Guid.NewGuid();

        /// <inheritdoc />
        public void Dispose()
        {
            _unsubscribeAction(this);
        }
    }
}
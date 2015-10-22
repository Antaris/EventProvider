namespace Antaris.EventProvider
{
    /// <summary>
    /// Defines the required contract for implementing an event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Gets the event aggregator.
        /// </summary>
        IEventProvider EventProvider { get; set; }
    }
}
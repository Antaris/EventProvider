namespace Antaris.EventProvider
{
    using System;

    /// <summary>
    /// Represents a weak reference to a target object allowing for garbage collection.
    /// </summary>
    /// <typeparam name="T">The reference type.</typeparam>
    public class WeakReference<T> : WeakReference
    {
        /// <summary>
        /// Initialises a new instance of <see cref="WeakReference{T}"/>.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        public WeakReference(T instance) : base(instance)
        {

        }

        /// <summary>
        /// Gets the target instance.
        /// </summary>
        public new T Target { get { return (T)base.Target; } }
    }
}
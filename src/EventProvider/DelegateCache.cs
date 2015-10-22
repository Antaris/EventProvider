namespace Antaris.EventProvider
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Provides a cache of delegates.
    /// </summary>
    public static class DelegateCache
    {
        private static readonly ConcurrentDictionary<MethodInfo, Delegate> _cache = new ConcurrentDictionary<MethodInfo, Delegate>();

        public static Delegate GetDelegate(MethodInfo openMethod, Type eventType, Type payloadType)
        {
            var closedMethod = openMethod.MakeGenericMethod(eventType);

            return _cache.GetOrAdd(closedMethod, (m) =>
            {
                var param = Expression.Parameter(eventType);
                var call = Expression.Call(param, closedMethod);
                var lambda = Expression.Lambda(call, param);

                return lambda.Compile();
            });
        }
    }
}
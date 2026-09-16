using System;
using System.Collections.Generic;

namespace Game.Core
{
    public static class GameEvents
    {
        private static readonly Dictionary<Type, Delegate> _subscribers = new();

        public static void Subscribe<TEvent>(Action<TEvent> handler)
        {
            var eventType = typeof(TEvent);
            _subscribers[eventType] = _subscribers.TryGetValue(eventType, out var existing)
                ? Delegate.Combine(existing, handler)
                : handler;
        }

        public static void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            var eventType = typeof(TEvent);
            if (!_subscribers.TryGetValue(eventType, out var existing))
            {
                return;
            }

            var combined = Delegate.Remove(existing, handler);
            if (combined == null)
            {
                _subscribers.Remove(eventType);
            }
            else
            {
                _subscribers[eventType] = combined;
            }
        }

        public static void Publish<TEvent>(TEvent eventData)
        {
            var eventType = typeof(TEvent);
            if (_subscribers.TryGetValue(eventType, out var existing) && existing is Action<TEvent> handler)
            {
                handler.Invoke(eventData);
            }
        }
    }
}

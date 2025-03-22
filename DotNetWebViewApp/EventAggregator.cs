using System.Collections.Concurrent;

namespace DotNetWebViewApp
{
    public class EventAggregator
    {
        private readonly ConcurrentDictionary<string, List<Action<object[]>>> eventSubscribers = new();

        public void Subscribe(string eventName, Action<object[]> handler)
        {
            if (!eventSubscribers.ContainsKey(eventName))
            {
                eventSubscribers[eventName] = new List<Action<object[]>>();
            }
            eventSubscribers[eventName].Add(handler);
            Logger.Info($"Subscribed to event: {eventName}");
        }

        public void Unsubscribe(string eventName, Action<object[]> handler)
        {
            if (eventSubscribers.TryGetValue(eventName, out var handlers))
            {
                handlers.Remove(handler);
                if (handlers.Count == 0)
                {
                    eventSubscribers.TryRemove(eventName, out _);
                }
                Logger.Info($"Unsubscribed from event: {eventName}");
            }
        }

        public void Publish(string eventName, params object[] args)
        {
            if (eventSubscribers.TryGetValue(eventName, out var handlers))
            {
                Logger.Info($"Publishing event: {eventName} with args: {string.Join(", ", args)}");
                foreach (var handler in handlers)
                {
                    handler.Invoke(args);
                }
            }
            else
            {
                Logger.Warning($"No subscribers for event: {eventName}");
            }
        }
    }
}

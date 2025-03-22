using System.Collections.Concurrent;

namespace DotNetWebViewApp
{
    /// <summary>
    /// Facilitates communication between the main process and renderer processes.
    /// Provides methods to register, handle, and emit events or invoke handlers for inter-process communication.
    /// </summary>
    public static class IpcMain
    {
        private static readonly ConcurrentDictionary<string, List<Delegate>> EventListeners = new();
        private static readonly ConcurrentDictionary<string, Func<object[], Task<object>>> InvokeHandlers = new();

        /// <summary>
        /// Registers an event listener for a specific channel.
        /// </summary>
        public static void On(string channel, Action<object[]> listener)
        {
            if (!EventListeners.ContainsKey(channel))
            {
                EventListeners[channel] = new List<Delegate>();
            }
            EventListeners[channel].Add(listener);
        }

        /// <summary>
        /// Adds a one-time listener for a specific channel.
        /// </summary>
        public static void Once(string channel, Action<object[]> listener)
        {
            Action<object[]> wrapper = null;
            wrapper = args =>
            {
                listener(args);
                Off(channel, wrapper);
            };
            On(channel, wrapper);
        }

        /// <summary>
        /// Removes a specific listener for a given channel.
        /// </summary>
        public static void Off(string channel, Action<object[]> listener)
        {
            if (EventListeners.TryGetValue(channel, out var listeners))
            {
                listeners.Remove(listener);
                if (listeners.Count == 0)
                {
                    EventListeners.TryRemove(channel, out _);
                }
            }
        }

        /// <summary>
        /// Removes all listeners for a specific channel or all channels if no channel is specified.
        /// </summary>
        public static void RemoveAllListeners(string channel = null)
        {
            if (channel == null)
            {
                EventListeners.Clear();
            }
            else
            {
                EventListeners.TryRemove(channel, out _);
            }
        }

        /// <summary>
        /// Adds a handler for an invokeable IPC.
        /// </summary>
        public static void Handle(string channel, Func<object[], Task<object>> handler)
        {
            InvokeHandlers[channel] = handler;
            Console.WriteLine($"Handler registered for channel: {channel}");
        }

        /// <summary>
        /// Adds a one-time handler for an invokeable IPC.
        /// </summary>
        public static void HandleOnce(string channel, Func<object[], Task<object>> handler)
        {
            Func<object[], Task<object>> wrapper = null;
            wrapper = async args =>
            {
                var result = await handler(args);
                RemoveHandler(channel);
                return result;
            };
            Handle(channel, wrapper);
        }

        /// <summary>
        /// Removes the handler for a specific channel.
        /// </summary>
        public static void RemoveHandler(string channel)
        {
            InvokeHandlers.TryRemove(channel, out _);
        }

        /// <summary>
        /// Emits an event to all listeners of a specific channel.
        /// </summary>
        public static void Emit(string channel, params object[] args)
        {
            if (EventListeners.TryGetValue(channel, out var listeners))
            {
                foreach (var listener in listeners)
                {
                    (listener as Action<object[]>)?.Invoke(args);
                }
            }
        }

        /// <summary>
        /// Invokes a handler for a specific channel and returns the result.
        /// </summary>
        public static async Task<object> Invoke(string channel, params object[] args)
        {
            if (InvokeHandlers.TryGetValue(channel, out var handler))
            {
                Logger.Info($"Invoking handler for channel: {channel} with args: {string.Join(", ", args)}");
                var result = await handler(args);
                Logger.Info($"Handler result for channel '{channel}': {result}");
                return result;
            }
            Logger.Warning($"No handler registered for channel: {channel}");
            throw new InvalidOperationException($"No handler registered for channel: {channel}");
        }

        /// <summary>
        /// Checks if a handler exists for a specific channel.
        /// </summary>
        public static bool HasHandler(string channel)
        {
            return InvokeHandlers.ContainsKey(channel);
        }

        /// <summary>
        /// Checks if there are any listeners for a specific channel.
        /// </summary>
        public static bool HasListeners(string channel)
        {
            return EventListeners.ContainsKey(channel) && EventListeners[channel].Count > 0;
        }
    }
}

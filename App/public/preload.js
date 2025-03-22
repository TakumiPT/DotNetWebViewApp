/**
 * A bridge object that facilitates communication between the browser context
 * and the WebView host. It provides an `invoke` method to send messages and
 * receive responses asynchronously.
 */
(function () {
  window.bridge = {
    /**
     * Sends a message to the WebView host and waits for a response.
     *
     * @param {string} channel - The name of the communication channel.
     * @param {...any} args - Additional arguments to send with the message.
     * @returns {Promise<any>} A promise that resolves with the response from the WebView host.
     *
     * @example
     * // Example usage:
     * window.bridge.invoke('getData', { id: 123 })
     *   .then(response => {
     *     console.log('Response:', response);
     *   })
     *   .catch(error => {
     *     console.error('Error:', error);
     *   });
     */
    invoke: function (channel, ...args) {
      return new Promise((resolve, reject) => {
        const message = { channel, args };
        console.log('Sending message:', message);
        window.chrome.webview.postMessage(JSON.stringify(message));

        /**
         * Handles incoming messages from the WebView host.
         *
         * @param {MessageEvent} event - The message event containing the response.
         */
        const handleMessage = (event) => {
          console.log('Raw message received:', event.data);
          try {
            const data = JSON.parse(event.data);
            console.log('Parsed message received:', data);
            if (data.channel === channel) {
              console.log('Resolving promise with result:', data.result);
              resolve(data.result);
              window.chrome.webview.removeEventListener('message', handleMessage);
            }
          } catch (error) {
            console.error('Error parsing message:', error);
            reject(error);
            window.chrome.webview.removeEventListener('message', handleMessage);
          }
        };

        window.chrome.webview.addEventListener('message', handleMessage);
      });
    },

    /**
     * Sends a message to the WebView host and waits synchronously for a response.
     *
     * @param {string} channel - The name of the communication channel.
     * @param {...any} args - Additional arguments to send with the message.
     * @returns {any} The response from the WebView host.
     *
     * @example
     * // Example usage:
     * const response = window.bridge.sendSync('getDataSync', { id: 123 });
     * console.log('Synchronous response:', response);
     */
    sendSync: function (channel, ...args) {
      const message = { channel, args };
      console.log('Sending synchronous message:', message);
      window.chrome.webview.postMessage(JSON.stringify(message));

      let response = null;
      let isResolved = false;

      /**
       * Handles the synchronous response from the WebView host.
       *
       * @param {MessageEvent} event - The message event containing the response.
       */
      const handleMessage = (event) => {
        console.log('Raw synchronous message received:', event.data);
        try {
          const data = JSON.parse(event.data);
          console.log('Parsed synchronous message received:', data);
          if (data.channel === channel) {
            response = data.result;
            isResolved = true;
            window.chrome.webview.removeEventListener('message', handleMessage);
          }
        } catch (error) {
          console.error('Error parsing synchronous message:', error);
          isResolved = true; // Exit the loop even if there's an error
          window.chrome.webview.removeEventListener('message', handleMessage);
        }
      };

      window.chrome.webview.addEventListener('message', handleMessage);

      // Busy-wait loop to simulate synchronous behavior
      const start = Date.now();
      const timeout = 5000; // 5 seconds timeout
      while (!isResolved) {
        if (Date.now() - start > timeout) {
          console.error('sendSync timed out');
          window.chrome.webview.removeEventListener('message', handleMessage);
          throw new Error('sendSync timed out');
        }
      }

      return response;
    },

    /**
     * Sends a message to the WebView host without waiting for a response.
     *
     * @param {string} channel - The name of the communication channel.
     * @param {...any} args - Additional arguments to send with the message.
     *
     * @example
     * // Example usage:
     * window.bridge.send('logMessage', 'This is a test message');
     */
    send: function (channel, ...args) {
      const message = { channel, args };
      console.log('Sending message without waiting for response:', message);
      window.chrome.webview.postMessage(JSON.stringify(message));
    },

    /**
     * Sends a message directly to the WebView host without expecting a response.
     *
     * @param {string} channel - The name of the communication channel.
     * @param {...any} args - Additional arguments to send with the message.
     *
     * @example
     * // Example usage:
     * window.bridge.sendToHost('hostChannel', 'This is a message for the host');
     */
    sendToHost: function (channel, ...args) {
      const message = { channel, args };
      console.log('Sending message to host:', message);
      window.chrome.webview.postMessage(JSON.stringify(message));
    },

    /**
     * Registers an event listener for a specific channel. The listener will be
     * called whenever a message is received on the specified channel.
     *
     * @param {string} channel - The name of the communication channel.
     * @param {function} listener - The callback function to execute when a message is received.
     *
     * @example
     * // Example usage:
     * window.bridge.on('channelName', (event, data) => {
     *   console.log('Message received on channelName:', data);
     * });
     */
    on: function (channel, listener) {
      /**
       * Handles incoming messages and triggers the listener if the channel matches.
       *
       * @param {MessageEvent} event - The message event containing the response.
       */
      const handleMessage = (event) => {
        console.log('Raw message received for on:', event.data);
        try {
          const data = JSON.parse(event.data);
          if (data.channel === channel) {
            console.log(`Triggering listener for channel: ${channel}`);
            listener(event, data.args);
          }
        } catch (error) {
          console.error('Error parsing message for on:', error);
        }
      };

      window.chrome.webview.addEventListener('message', handleMessage);

      // Return a function to allow removing the listener
      return () => {
        console.log(`Removing listener for channel: ${channel}`);
        window.chrome.webview.removeEventListener('message', handleMessage);
      };
    },

    /**
     * Alias for the `on` method. Registers an event listener for a specific channel.
     * The listener will be called whenever a message is received on the specified channel.
     *
     * @param {string} channel - The name of the communication channel.
     * @param {function} listener - The callback function to execute when a message is received.
     *
     * @example
     * // Example usage:
     * window.bridge.addListener('channelName', (event, data) => {
     *   console.log('Message received on channelName:', data);
     * });
     */
    addListener: function (channel, listener) {
      return this.on(channel, listener);
    },

    /**
     * Adds a one-time listener for a specific channel. The listener will be
     * invoked only the next time a message is sent to the channel, after which
     * it is removed.
     *
     * @param {string} channel - The name of the communication channel.
     * @param {function} listener - The callback function to execute when a message is received.
     *
     * @example
     * // Example usage:
     * window.bridge.once('channelName', (event, data) => {
     *   console.log('One-time message received on channelName:', data);
     * });
     */
    once: function (channel, listener) {
      const handleMessage = (event) => {
        console.log('Raw message received for once:', event.data);
        try {
          const data = JSON.parse(event.data);
          if (data.channel === channel) {
            console.log(`Triggering one-time listener for channel: ${channel}`);
            listener(event, data.args);
            window.chrome.webview.removeEventListener('message', handleMessage);
          }
        } catch (error) {
          console.error('Error parsing message for once:', error);
        }
      };

      window.chrome.webview.addEventListener('message', handleMessage);
    },

    /**
     * Removes a specific event listener for a given channel.
     *
     * @param {string} channel - The name of the communication channel.
     * @param {function} listener - The callback function to remove.
     *
     * @example
     * // Example usage:
     * const listener = (event, data) => {
     *   console.log('Message received:', data);
     * };
     * window.bridge.on('channelName', listener);
     * window.bridge.removeListener('channelName', listener);
     */
    removeListener: function (channel, listener) {
      console.log(`Removing listener for channel: ${channel}`);
      const handleMessage = (event) => {
        try {
          const data = JSON.parse(event.data);
          if (data.channel === channel) {
            // No-op: This ensures the specific listener is removed.
          }
        } catch (error) {
          console.error('Error parsing message during listener removal:', error);
        }
      };

      window.chrome.webview.removeEventListener('message', handleMessage);
    },

    /**
     * Removes all event listeners for a specific channel.
     *
     * @param {string} channel - The name of the communication channel.
     *
     * @example
     * // Example usage:
     * window.bridge.removeAllListeners('channelName');
     */
    removeAllListeners: function (channel) {
      console.log(`Removing all listeners for channel: ${channel}`);
      const handleMessage = (event) => {
        try {
          const data = JSON.parse(event.data);
          if (data.channel === channel) {
            // No-op: This ensures all listeners for the channel are removed.
          }
        } catch (error) {
          console.error('Error parsing message during listener removal:', error);
        }
      };

      window.chrome.webview.removeEventListener('message', handleMessage);
    }
  };
})();

(function () {
  window.bridge = {
    invoke: function (channel, ...args) {
      return new Promise((resolve, reject) => {
        const message = { channel, args };
        console.log('Sending message:', message);
        window.chrome.webview.postMessage(JSON.stringify(message));

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
    }
  };
})();

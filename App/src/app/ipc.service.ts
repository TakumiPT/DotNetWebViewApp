import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class IpcService {
  invoke(channel: string, ...args: any[]): Promise<any> {
    return new Promise((resolve, reject) => {
      const message = { channel, args };
      console.log('Sending message:', message);
      (window as any).chrome.webview.postMessage(JSON.stringify(message));
      
      const timeoutId = setTimeout(() => {
        reject(new Error('Timeout waiting for response'));
        (window as any).chrome.webview.removeEventListener('message', handleMessage);
      }, 5000); // 5 seconds timeout

      const handleMessage = (event: any) => {
        console.log('Raw message received in Angular:', event.data);
        try {
          const data = JSON.parse(event.data);
          console.log('Parsed message received in Angular:', data);
          if (data.channel === channel) {
            console.log('Resolving promise with result:', data.result);
            clearTimeout(timeoutId); // Clear the timeout
            resolve(data.result);
            (window as any).chrome.webview.removeEventListener('message', handleMessage);
          }
        } catch (error) {
          console.error('Error parsing message:', error);
          clearTimeout(timeoutId); // Clear the timeout
          reject(error);
          (window as any).chrome.webview.removeEventListener('message', handleMessage);
        }
      };
      (window as any).chrome.webview.addEventListener('message', handleMessage);
    });
  }
}

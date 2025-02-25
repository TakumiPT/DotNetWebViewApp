import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BridgeService {
  invoke(channel: string, ...args: any[]): Promise<any> {
    if (!(window as any).bridge) {
      throw new Error('Bridge is not available');
    }
    return (window as any).bridge.invoke(channel, ...args);
  }
}

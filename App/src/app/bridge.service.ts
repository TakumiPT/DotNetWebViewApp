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

  readFile(path: string): Promise<string> {
    return this.invoke('readFile', path);
  }

  saveFile(path: string, data: string): Promise<void> {
    return this.invoke('saveFile', path, data);
  }

  readdir(path: string): Promise<string[]> {
    return this.invoke('readdir', path);
  }

  showOpenDialog(): Promise<string> {
    return this.invoke('showOpenDialog');
  }

  showSaveDialog(): Promise<string> {
    return this.invoke('showSaveDialog');
  }

  showMessageBox(message: string): Promise<void> {
    return this.invoke('showMessageBox', message);
  }

  getToken(): Promise<string> {
    return this.invoke('getToken');
  }

  getAuthProfile(): Promise<any> {
    return this.invoke('getAuthProfile');
  }

  closeMainWindow(): Promise<void> {
    return this.invoke('closeMainWindow');
  }

  getAppVersion(): Promise<string> {
    return this.invoke('getAppVersion');
  }

  getAppPlatform(): Promise<string> {
    return this.invoke('getAppPlatform');
  }

  runCommand(command: string): Promise<any> {
    return this.invoke('runCommand', command);
  }

  getUserHost(): Promise<{ username: string, hostname: string }> {
    return this.invoke('getUserHost');
  }
}

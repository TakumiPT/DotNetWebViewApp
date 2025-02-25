import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { BridgeService } from './bridge.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'AngularApp';

  constructor(private bridgeService: BridgeService) {}

  ngOnInit() {
  }

  async getVersion() {
    try {
      const version = await this.bridgeService.invoke('version');
      console.log(`App version: ${version}`);
      alert(`App version: ${version}`);
    } catch (error) {
      console.error('Error getting version:', error);
    }
  }

  async getStatus() {
    try {
      const status = await this.bridgeService.invoke('status');
      console.log(`App status: ${status}`);
      alert(`App status: ${status}`);
    } catch (error) {
      console.error('Error getting status:', error);
    }
  }

  async getPlatform() {
    try {
      const platform = await this.bridgeService.invoke('platform');
      console.log(`App platform: ${platform}`);
      alert(`App platform: ${platform}`);
    } catch (error) {
      console.error('Error getting platform:', error);
    }
  }

  async openFolderDialog() {
    try {
      const selectedPath = await this.bridgeService.invoke('openFolderDialog');
      console.log(`Selected folder path: ${selectedPath}`);
      alert(`Selected folder path: ${selectedPath}`);
    } catch (error) {
      console.error('Error opening folder dialog:', error);
    }
  }

  async readFile() {
    try {
      const filePath = prompt('Enter the file path:');
      if (filePath) {
        const content = await this.bridgeService.invoke('readFile', filePath);
        console.log(`File content: ${content}`);
        alert(`File content: ${content}`);
      }
    } catch (error) {
      console.error('Error reading file:', error);
    }
  }

  async saveFile() {
    try {
      const filePath = prompt('Enter the file path:');
      const content = prompt('Enter the file content:');
      if (filePath && content) {
        await this.bridgeService.invoke('saveFile', filePath, content);
        console.log('File saved successfully');
        alert('File saved successfully');
      }
    } catch (error) {
      console.error('Error saving file:', error);
    }
  }

  async readDir() {
    try {
      const dirPath = prompt('Enter the directory path:');
      if (dirPath) {
        const files = await this.bridgeService.invoke('readdir', dirPath);
        console.log(`Directory content: ${files}`);
        alert(`Directory content: ${files}`);
      }
    } catch (error) {
      console.error('Error reading directory:', error);
    }
  }

  async showMessageBox() {
    try {
      const message = prompt('Enter the message:');
      if (message) {
        await this.bridgeService.invoke('showMessageBox', message);
        console.log('Message box shown');
        alert('Message box shown');
      }
    } catch (error) {
      console.error('Error showing message box:', error);
    }
  }

  async getToken() {
    try {
      const token = await this.bridgeService.invoke('getToken');
      console.log(`Token: ${token}`);
      alert(`Token: ${token}`);
    } catch (error) {
      console.error('Error getting token:', error);
    }
  }

  async getAuthProfile() {
    try {
      const profile = await this.bridgeService.invoke('getAuthProfile');
      console.log(`Auth profile: ${profile}`);
      alert(`Auth profile: ${profile}`);
    } catch (error) {
      console.error('Error getting auth profile:', error);
    }
  }

  async closeMainWindow() {
    try {
      await this.bridgeService.invoke('closeMainWindow');
      console.log('Main window closed');
      alert('Main window closed');
    } catch (error) {
      console.error('Error closing main window:', error);
    }
  }

  async runCommand() {
    try {
      const command = prompt('Enter the command:');
      if (command) {
        const result = await this.bridgeService.invoke('runCommand', command);
        console.log(`Command result: ${result}`);
        alert(`Command result: ${result}`);
      }
    } catch (error) {
      console.error('Error running command:', error);
    }
  }

  async getUserHost() {
    try {
      const userHost = await this.bridgeService.invoke('getUserHost');
      console.log(`User host: ${userHost}`);
      alert(`User host: ${userHost}`);
    } catch (error) {
      console.error('Error getting user host:', error);
    }
  }
}

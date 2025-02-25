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
}

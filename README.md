# DotNetWebViewApp

## Overview

DotNetWebViewApp is a hybrid application that integrates a .NET backend with an Angular frontend using WebView2. The project enables seamless bidirectional communication between the backend and frontend through IPC (Inter-Process Communication) using `IpcMain`.

This document explains the project structure, how the .NET and Angular applications communicate, and how to extend the application by adding new `IpcMain` handlers.

---

## Project Structure

### Backend (.NET)
- **`Controllers`**: Contains controllers like `AppController` to handle IPC requests from the frontend.
- **`Services`**: Contains services like `AppService` to encapsulate business logic.
- **`IpcHandlerFactory.cs`**: Centralized factory for registering all `IpcMain` handlers.
- **`WebViewSettingsBuilder.cs`**: Configures WebView2 settings.

### Frontend (Angular)
- **`src/app`**: Contains Angular components and services.
- **`bridge.service.ts`**: Provides a wrapper for invoking IPC handlers from the Angular application.

---

## Communication Between .NET and Angular

### How It Works
1. **Frontend to Backend Communication**:
   - The Angular application uses the `BridgeService` to invoke IPC handlers.
   - Example: 
     ```typescript
     const result = await this.bridgeService.invoke('version');
     ```
   - This sends a message to the .NET backend via `IpcMain.Handle`.

2. **Backend to Frontend Communication**:
   - The .NET backend registers handlers using `IpcMain.Handle` in `IpcHandlerFactory.cs`.
   - Example:
     ```csharp
     IpcMain.Handle("version", async args =>
     {
         return await Task.FromResult(Application.ProductVersion);
     });
     ```
   - The handler processes the request and returns a response to the Angular frontend.

---

## Adding New `IpcMain` Handlers

To add a new IPC handler, follow these steps:

### 1. Define the Handler in `IpcHandlerFactory.cs`
Add a new handler in the `RegisterHandlers` method of `IpcHandlerFactory.cs`.

Example:
```csharp
// filepath: DotNetWebViewApp\IpcHandlerFactory.cs
Logger.Debug("Registering handler for: newFunction");
IpcMain.Handle("newFunction", async args =>
{
    Logger.Info("Handler invoked: newFunction");
    // Add your logic here
    return await Task.FromResult("Response from newFunction");
});
```

### 2. Invoke the Handler from Angular
Use the `BridgeService` in Angular to invoke the new handler.

Example:
```typescript
// filepath: App\src\app\app.component.ts
async callNewFunction() {
  try {
    const response = await this.bridgeService.invoke('newFunction');
    console.log(`Response from newFunction: ${response}`);
  } catch (error) {
    console.error('Error calling newFunction:', error);
  }
}
```

### 3. Add a Button in the Angular Template
Add a button in the Angular template to trigger the new function.

Example:
```html
<!-- filepath: App\src\app\app.component.html -->
<button (click)="callNewFunction()">Call New Function</button>
```

---

## Example: Adding a `getSystemInfo` Handler

### Backend
Add the following handler in `IpcHandlerFactory.cs`:
```csharp
// filepath: DotNetWebViewApp\IpcHandlerFactory.cs
Logger.Debug("Registering handler for: getSystemInfo");
IpcMain.Handle("getSystemInfo", async args =>
{
    Logger.Info("Handler invoked: getSystemInfo");
    var systemInfo = new
    {
        OS = Environment.OSVersion.ToString(),
        ProcessorCount = Environment.ProcessorCount,
        MachineName = Environment.MachineName
    };
    return await Task.FromResult(System.Text.Json.JsonSerializer.Serialize(systemInfo));
});
```

### Frontend
Add the following method in `app.component.ts`:
```typescript
// filepath: App\src\app\app.component.ts
async getSystemInfo() {
  try {
    const systemInfo = await this.bridgeService.invoke('getSystemInfo');
    console.log(`System Info: ${systemInfo}`);
    alert(`System Info: ${systemInfo}`);
  } catch (error) {
    console.error('Error getting system info:', error);
  }
}
```

Add a button in `app.component.html`:
```html
<!-- filepath: App\src\app\app.component.html -->
<button (click)="getSystemInfo()">Get System Info</button>
```

---

## Debugging Tips

1. **Check Logs**:
   - Backend logs are written using `Logger.Debug` and `Logger.Info`.
   - Check for errors in the console output.

2. **Test IPC Handlers**:
   - Use the Angular `BridgeService` to test IPC handlers.
   - Ensure the handler name matches between the frontend and backend.

3. **Error Handling**:
   - Use `try-catch` blocks in both the backend and frontend to handle errors gracefully.

---

## Conclusion

This project provides a robust framework for integrating a .NET backend with an Angular frontend using WebView2. By following the steps above, you can easily add new IPC handlers and extend the application's functionality.

Let me know if you need further assistance!

## About Me

Hi, I'm **Filipe Freire**, a software developer with a strong interest in building hybrid applications that bridge the gap between web and desktop technologies. While my expertise spans multiple areas, this project reflects my ability to learn and apply .NET alongside Angular to create scalable and responsive solutions. Feel free to connect with me on [LinkedIn](https://www.linkedin.com/in/filipe-freire-45300738/) or explore more of my work on [GitHub](https://github.com/TakumiPT).

## Why This Project?

This project explores the integration of modern web technologies into traditional desktop applications, offering a practical example of how to bridge the gap between web and desktop development. It is designed to inspire developers, support teams exploring hybrid solutions, and demonstrate the potential of combining .NET and Angular for creating scalable, responsive applications.

## Key Features

- **Hybrid Application**: Combines a .NET Windows Forms backend with an Angular frontend.
- **WebView2 Integration**: Hosts a modern web application inside a desktop application.
- **Custom Communication Bridge**: Enables seamless communication between the .NET backend and the Angular frontend.
- **Extensible Architecture**: Designed for scalability with plans to implement modular controllers for better separation of concerns.

## Technologies Used

- **.NET 6.0+**: For building the Windows Forms application.
- **WebView2**: To embed the Angular application within the desktop app.
- **Angular**: For creating a dynamic and responsive frontend.
- **TypeScript**: For type-safe frontend development.
- **C#**: For backend logic and communication handling.

## Setup

### Prerequisites

- .NET 6.0 SDK or later
- Node.js and npm
- Angular CLI

### Steps

1. Clone the repository:

    ```sh
    git clone https://github.com/TakumiPT/DotNetWebViewApp.git
    cd DotNetWebViewApp
    ```

2. Install Angular dependencies:

    ```sh
    cd App
    npm install
    ```

3. Build the Angular application:

    ```sh
    ng build
    ```

4. Run the .NET application:

    ```sh
    cd ..
    dotnet run --project DotNetWebViewApp
    ```

## How It Works

### Angular Frontend

The Angular frontend provides a modern, responsive UI and includes buttons to trigger various backend functionalities, such as:
- Retrieving application version and status.
- Performing file operations (read, save, open folder dialog).
- Displaying system information (platform, user, hostname).
- Running commands and managing user authentication.

### .NET Backend

The .NET backend handles requests from the Angular frontend and performs the corresponding actions. Handlers are defined in the `Form1.cs` file and will be modularized into controllers in future implementations.

### Communication Bridge

The communication between the Angular frontend and the .NET backend is facilitated by a custom bridge implemented in the `preload.js` file. This bridge exposes an `invoke` method that allows the Angular frontend to send messages to the .NET backend.

### Adding New Handlers

To add new handlers:
1. Define the handler method in the backend (e.g., `Form1.cs` or a controller).
2. Register the handler using `IpcMain.Handle`.
3. Call the handler from the Angular frontend using the `BridgeService`.

Example:

```csharp
// Backend: Define the handler
private string HandleCustomRequest(string[] args)
{
    return "Custom functionality executed";
}

// Register the handler
IpcMain.Handle("customRequest", async args =>
{
    return await Task.FromResult(HandleCustomRequest(args));
});
```

```typescript
// Frontend: Call the handler
this.bridgeService.invoke('customRequest', arg1, arg2).then(result => {
  console.log(result);
});
```

## Future Plans: Modular Controllers

To improve maintainability and scalability, future implementations will include **modular controllers**. Each controller will handle a specific domain (e.g., file operations, user management, system commands).

### Benefits:
- **Separation of Concerns**: Each controller focuses on a specific functionality.
- **Improved Readability**: Reduces complexity in the `Form1.cs` file.
- **Easier Testing**: Controllers can be tested independently.
- **Scalability**: Adding new features becomes easier.

### Example Structure:
- `Controllers/FileController.cs`: Handles file-related operations.
- `Controllers/UserController.cs`: Manages user authentication and profiles.
- `Controllers/SystemController.cs`: Handles system-level commands and information.

### Example Usage:
```csharp
// FileController.cs
public class FileController
{
    public string ReadFile(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    public void SaveFile(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
    }
}
```

```csharp
// Form1.cs
private void RegisterIpcMainHandlers()
{
    var fileController = new FileController();

    IpcMain.Handle("readFile", async args =>
    {
        string filePath = args[0]?.ToString();
        return await Task.Run(() => fileController.ReadFile(filePath));
    });

    IpcMain.Handle("saveFile", async args =>
    {
        string filePath = args[0]?.ToString();
        string content = args[1]?.ToString();
        await Task.Run(() => fileController.SaveFile(filePath, content));
        return "File saved successfully";
    });
}
```

## Why This Project Stands Out

This project demonstrates:
- **Versatility**: The ability to seamlessly integrate backend and frontend technologies into a cohesive solution.
- **Modern Desktop Integration**: Utilizes WebView2 to embed web applications within a desktop environment, bridging the gap between traditional and modern development paradigms.
- **Clean Code Practices**: Follows established design principles and patterns (e.g., Singleton, modular controllers) to ensure maintainability and readability.
- **Scalability**: Built with future extensibility in mind, making it adaptable to evolving requirements.
- **Innovative Problem-Solving**: Showcases the integration of web technologies into desktop applications, addressing real-world challenges with hybrid solutions.

## About the Author

This project was developed by **Filipe Freire**, a software developer with a strong interest in hybrid application development. While I am not a .NET expert, this project reflects my ability to learn and apply new technologies effectively. Feel free to connect with me on [LinkedIn](https://www.linkedin.com/in/filipe-freire-45300738/) or explore more of my work on [GitHub](https://github.com/TakumiPT).

## License

This project is licensed under the **MIT License**. You are free to use, modify, and distribute this project, provided that proper attribution is given to the author, **Filipe Freire**. See the [LICENSE](./LICENSE) file for more details.

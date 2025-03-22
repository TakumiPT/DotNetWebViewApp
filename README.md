# DotNetWebViewApp

DotNetWebViewApp is a hybrid desktop application that integrates a .NET Windows Forms backend with an Angular frontend using WebView2. This project demonstrates how to combine the power of .NET for backend operations with the flexibility of Angular for frontend development. It serves as a showcase of my skills in full-stack development, clean code practices, and modern desktop application design.

## About Me

Hi, I'm **Filipe Freire**, a passionate software developer with expertise in .NET, Angular, and hybrid application development. I created this project to demonstrate my ability to integrate modern web technologies into traditional desktop applications. Feel free to connect with me on [LinkedIn](https://www.linkedin.com/in/filipe-freire-45300738/) or explore more of my work on [GitHub](https://github.com/TakumiPT).

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

## Why Recruiters Should Care

This project showcases:
- **Full-Stack Development**: Combines backend (C#) and frontend (Angular) technologies.
- **Modern Desktop Integration**: Demonstrates the use of WebView2 for embedding web applications in desktop apps.
- **Clean Code Practices**: Adheres to clean code principles and design patterns (e.g., Singleton, modular controllers).
- **Scalability**: Designed with future extensibility in mind.
- **Problem-Solving Skills**: Highlights the ability to integrate modern web technologies into traditional desktop applications.

## About the Author

This project was developed by **Filipe Freire**, a passionate software developer with expertise in .NET, Angular, and hybrid application development. Feel free to connect with me on [LinkedIn](https://www.linkedin.com/in/filipe-freire-45300738/) or explore more of my work on [GitHub](https://github.com/TakumiPT).

## License

This project is licensed under the MIT License.

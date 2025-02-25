# DotNetWebViewApp

This project demonstrates how to integrate a .NET Windows Forms application with an Angular frontend using WebView2. The application allows communication between the .NET backend and the Angular frontend through a bridge.

## Features

- Host an Angular application inside a .NET Windows Forms application using WebView2.
- Communicate between the .NET backend and the Angular frontend using a custom bridge.
- Handle various functionalities such as reading files, saving files, opening dialogs, and more.

## Setup

### Prerequisites

- .NET 6.0 SDK or later
- Node.js and npm
- Angular CLI

### Steps

1. Clone the repository:

    ```sh
    git clone https://github.com/your-repo/DotNetWebViewApp.git
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

## Usage

### Angular Frontend

The Angular frontend includes a set of buttons to trigger various functionalities provided by the .NET backend. These functionalities include:

- Get Version
- Get Status
- Get Platform
- Open Folder Dialog
- Read File
- Save File
- Read Directory
- Show Message Box
- Get Token
- Get Auth Profile
- Close Main Window
- Run Command
- Get User Host

### .NET Backend

The .NET backend handles the requests from the Angular frontend and performs the corresponding actions. The handlers are defined in the `Form1.cs` file.

### Communication Bridge

The communication between the Angular frontend and the .NET backend is facilitated by a custom bridge implemented in the `preload.js` file. The bridge exposes an `invoke` method that allows the Angular frontend to send messages to the .NET backend.

### Adding New Handlers

To add new handlers, follow these steps:

1. Define the handler method in the `Form1.cs` file.
2. Add the handler to the `channelHandlers` dictionary in the `InitializeChannelHandlers` method.
3. Call the handler from the Angular frontend using the `BridgeService`.

Example:

```csharp
// Define the handler method in Form1.cs
private string HandleNewFunctionalityRequest(string[] args)
{
    // Implement your logic here
    return "New functionality executed";
}

// Add the handler to the channelHandlers dictionary
channelHandlers.Add("newFunctionality", HandleNewFunctionalityRequest);
```

```typescript
// Call the handler from the Angular frontend
this.bridgeService.invoke('newFunctionality', arg1, arg2).then(result => {
  console.log(result);
});
```

## License

This project is licensed under the MIT License.

# DotNetWebViewApp

## Prerequisites
- .NET SDK
- Node.js and npm (for Angular project)

## Running the Project

1. Open a terminal or command prompt.
2. Restore the project dependencies:
    ```sh
    dotnet restore
    ```
3. Build the project:
    ```sh
    dotnet build
    ```
4. Run the Angular project:
    ```sh
    cd App
    npm install
    ng serve
    ```
5. Run the .NET project:
    ```sh
    cd ..
    dotnet run
    ```

## Testing invoke

1. Use the buttons in the .NET application to invoke different channels (`version`, `status`, `platform`, `openFolderDialog`).
2. The Angular application will send messages to the .NET application and display the results in alerts.
3. The console in both the Angular and .NET applications should log the communication details.

The application should now be running and you can see the WebView2 control displaying the Angular application.

# DotNetWebViewApp

## Prerequisites
- .NET SDK
- Node.js and npm (for Angular project)

## Running the Project

1. Open a terminal or command prompt.
2. Navigate to the project directory:
    ```sh
    cd /c:/Users/fmiguelf/work/Teste
    ```
3. Restore the project dependencies:
    ```sh
    dotnet restore
    ```
4. Build the project:
    ```sh
    dotnet build
    ```
5. Run the Angular project:
    ```sh
    cd /c:/Users/fmiguelf/work/Teste/AngularApp
    npm install
    npm start
    ```
6. Run the .NET project:
    ```sh
    cd /c:/Users/fmiguelf/work/Teste
    dotnet run
    ```

## Testing invoke

1. The Angular application will automatically invoke the `alert` channel with the message "Hello from Angular!".
2. An alert should appear in the .NET application with the message "Hello from Angular!".
3. The console in the Angular application should log "Alert displayed".

The application should now be running and you can see the WebView2 control displaying the Angular application.

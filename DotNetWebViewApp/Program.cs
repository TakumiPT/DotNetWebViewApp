namespace DotNetWebViewApp;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        GlobalErrorHandler.RegisterGlobalExceptionHandler(); // Register global error handler
        Logger.Info("Application started."); // Test log message

        // Set the default icon globally
        ApplicationConfiguration.Initialize();
        Application.ApplicationExit += (sender, e) => Logger.Info("Application exited.");
        SetGlobalIcon();

        Application.Run(new Form1());
    }

    private static void SetGlobalIcon()
    {
        string faviconPath = Path.Combine(AppContext.BaseDirectory, ConfigurationManager.BrowserFolder, ConfigurationManager.FaviconFile);
        if (File.Exists(faviconPath))
        {
            Icon appIcon = new Icon(faviconPath);
            Application.AddMessageFilter(new IconMessageFilter(appIcon));
        }
        else
        {
            Logger.Warning("Favicon file not found. Default icon will be used.");
        }
    }
}
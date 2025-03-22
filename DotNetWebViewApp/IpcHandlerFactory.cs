namespace DotNetWebViewApp
{
    public static class IpcHandlerFactory
    {
        public static void RegisterHandlers()
        {
            Logger.Debug("Registering handler for: version");
            IpcMain.Handle("version", async args =>
            {
                Logger.Info("Handler invoked: version");
                return await Task.FromResult(Application.ProductVersion);
            });

            Logger.Debug("Registering handler for: status");
            IpcMain.Handle("status", async args =>
            {
                Logger.Info("Handler invoked: status");
                return await Task.FromResult("Running");
            });

            Logger.Debug("Registering handler for: platform");
            IpcMain.Handle("platform", async args =>
            {
                Logger.Info("Handler invoked: platform");
                return await Task.FromResult(Environment.OSVersion.Platform.ToString());
            });

            Logger.Debug("Registering handler for: openFolderDialog");
            IpcMain.Handle("openFolderDialog", async args =>
            {
                Logger.Info("Handler invoked: openFolderDialog");
                return await Task.Run(() => OpenFolderDialog());
            });

            Logger.Info("IpcMain handlers registered.");
        }

        private static string OpenFolderDialog()
        {
            using var dialog = new FolderBrowserDialog();
            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
        }
    }
}

namespace DotNetWebViewApp
{
    public sealed class IpcMainSingleton
    {
        private static readonly Lazy<IpcMainSingleton> instance = new(() => new IpcMainSingleton());

        public static IpcMainSingleton Instance => instance.Value;

        private IpcMainSingleton() { }

        public void RegisterHandlers()
        {
            Logger.Debug("Registering IPC handlers."); // Simplified log
            IpcMain.Handle("version", async args =>
            {
                Logger.Debug("Handler invoked: version"); // Retain meaningful log
                return await Task.FromResult(Application.ProductVersion);
            });

            IpcMain.Handle("status", async args =>
            {
                Logger.Debug("Handler invoked: status"); // Retain meaningful log
                return await Task.FromResult("Running");
            });

            IpcMain.Handle("platform", async args =>
            {
                Logger.Debug("Handler invoked: platform"); // Retain meaningful log
                return await Task.FromResult(Environment.OSVersion.Platform.ToString());
            });

            IpcMain.Handle("openFolderDialog", async args =>
            {
                Logger.Debug("Handler invoked: openFolderDialog"); // Retain meaningful log
                return await Task.Run(() => OpenFolderDialog());
            });

            Logger.Info("All IPC handlers registered."); // Retain meaningful log
        }

        public bool HasHandler(string channel)
        {
            bool hasHandler = IpcMain.HasHandler(channel);
            Logger.Debug($"HasHandler check for channel '{channel}': {hasHandler}");
            return hasHandler;
        }

        public object Invoke(string channel, params object[] args)
        {
            Logger.Debug($"Invoking handler for channel: {channel} with args: {string.Join(", ", args)}");
            return IpcMain.Invoke(channel, args).Result; // Synchronously wait for the result
        }

        private string OpenFolderDialog()
        {
            using var dialog = new FolderBrowserDialog();
            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
        }
    }
}

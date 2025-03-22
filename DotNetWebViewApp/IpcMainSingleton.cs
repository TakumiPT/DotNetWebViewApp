namespace DotNetWebViewApp
{
    public sealed class IpcMainSingleton
    {
        private static readonly Lazy<IpcMainSingleton> instance = new(() => new IpcMainSingleton());

        public static IpcMainSingleton Instance => instance.Value;

        private IpcMainSingleton() { }

        public void RegisterHandlers()
        {
            IpcMain.Handle("version", async args =>
            {
                Console.WriteLine("Handler invoked: version");
                return await Task.FromResult(Application.ProductVersion);
            });

            IpcMain.Handle("status", async args =>
            {
                Console.WriteLine("Handler invoked: status");
                return await Task.FromResult("Running");
            });

            IpcMain.Handle("platform", async args =>
            {
                Console.WriteLine("Handler invoked: platform");
                return await Task.FromResult(Environment.OSVersion.Platform.ToString());
            });

            IpcMain.Handle("openFolderDialog", async args =>
            {
                Console.WriteLine("Handler invoked: openFolderDialog");
                return await Task.Run(() => OpenFolderDialog());
            });

            Console.WriteLine("IpcMain handlers registered.");
        }

        public bool HasHandler(string channel)
        {
            bool hasHandler = IpcMain.HasHandler(channel);
            Console.WriteLine($"HasHandler check for channel '{channel}': {hasHandler}");
            return hasHandler;
        }

        public object Invoke(string channel, params object[] args)
        {
            Console.WriteLine($"Invoking handler for channel: {channel} with args: {string.Join(", ", args)}");
            return IpcMain.Invoke(channel, args).Result; // Synchronously wait for the result
        }

        private string OpenFolderDialog()
        {
            using var dialog = new FolderBrowserDialog();
            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
        }
    }
}

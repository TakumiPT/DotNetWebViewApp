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

        public string Emit(string channel, string[] args)
        {
            Console.WriteLine($"Emitting event: {channel} with args: {string.Join(", ", args)}");

            // Return a placeholder response for now
            return $"Event {channel} handled with args: {string.Join(", ", args)}";
        }

        public bool HasHandler(string channel)
        {
            return IpcMain.HasHandler(channel);
        }

        public object Invoke(string channel, params object[] args)
        {
            return IpcMain.Invoke(channel, args).Result; // Synchronously wait for the result
        }

        private string OpenFolderDialog()
        {
            using var dialog = new FolderBrowserDialog();
            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
        }
    }
}

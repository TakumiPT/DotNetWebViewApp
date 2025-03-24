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
            IpcMain.Handle("openFolderDialog", args =>
            {
                Logger.Info("Handler invoked: openFolderDialog");
                try
                {
                    string selectedPath = string.Empty;

                    using var dialog = new FolderBrowserDialog();
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        selectedPath = dialog.SelectedPath;
                    }

                    Logger.Info($"Folder selected: {selectedPath}");
                    return Task.FromResult<object>(selectedPath);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error in openFolderDialog: {ex.Message}");
                    return Task.FromResult<object>(string.Empty); // Return an empty string in case of an error
                }
            });

            Logger.Debug("Registering handler for: readFile");
            IpcMain.Handle("readFile", async args =>
            {
                Logger.Info("Handler invoked: readFile");
                string filePath = args[0]?.ToString();
                return await Task.Run(() => File.ReadAllText(filePath));
            });

            Logger.Debug("Registering handler for: saveFile");
            IpcMain.Handle("saveFile", async args =>
            {
                Logger.Info("Handler invoked: saveFile");
                string filePath = args[0]?.ToString();
                string content = args[1]?.ToString();
                await Task.Run(() => File.WriteAllText(filePath, content));
                return "File saved successfully";
            });

            Logger.Debug("Registering handler for: readdir");
            IpcMain.Handle("readdir", async args =>
            {
                Logger.Info("Handler invoked: readdir");
                string dirPath = args[0]?.ToString();
                return await Task.Run(() => Directory.GetFiles(dirPath));
            });

            Logger.Debug("Registering handler for: showMessageBox");
            IpcMain.Handle("showMessageBox", async args =>
            {
                Logger.Info("Handler invoked: showMessageBox");
                string message = args[0]?.ToString();
                await Task.Run(() => MessageBox.Show(message, "Message Box"));
                return "Message shown";
            });

            Logger.Debug("Registering handler for: getToken");
            IpcMain.Handle("getToken", async args =>
            {
                Logger.Info("Handler invoked: getToken");
                return await Task.FromResult("SampleToken123");
            });

            Logger.Debug("Registering handler for: getAuthProfile");
            IpcMain.Handle("getAuthProfile", async args =>
            {
                Logger.Info("Handler invoked: getAuthProfile");
                var profile = new { Username = "User123", Email = "user@example.com" };
                return await Task.FromResult(System.Text.Json.JsonSerializer.Serialize(profile));
            });

            Logger.Debug("Registering handler for: closeMainWindow");
            IpcMain.Handle("closeMainWindow", async args =>
            {
                Logger.Info("Handler invoked: closeMainWindow");
                await Task.Run(() => Application.Exit());
                return "Main window closed";
            });

            Logger.Debug("Registering handler for: runCommand");
            IpcMain.Handle("runCommand", async args =>
            {
                Logger.Info("Handler invoked: runCommand");
                try
                {
                    string command = args[0]?.ToString();
                    if (string.IsNullOrWhiteSpace(command))
                    {
                        throw new ArgumentException("Command cannot be null or empty.");
                    }

                    string result = await Task.Run(() => ExecuteCommand(command));
                    Logger.Info($"Command executed successfully: {command}");
                    return result;
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error in runCommand: {ex.Message}");
                    return $"Error: {ex.Message}";
                }
            });

            Logger.Debug("Registering handler for: getUserHost");
            IpcMain.Handle("getUserHost", async args =>
            {
                Logger.Info("Handler invoked: getUserHost");
                var userHost = new
                {
                    Username = Environment.UserName,
                    Hostname = Environment.MachineName
                };
                return await Task.FromResult(System.Text.Json.JsonSerializer.Serialize(userHost));
            });

            Logger.Info("IpcMain handlers registered.");
        }

        private static string OpenFolderDialog()
        {
            using var dialog = new FolderBrowserDialog();
            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
        }

        private static string ExecuteCommand(string command)
        {
            try
            {
                var processStartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = System.Diagnostics.Process.Start(processStartInfo);
                if (process == null)
                {
                    throw new InvalidOperationException("Failed to start process.");
                }

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                return string.IsNullOrWhiteSpace(error) ? output : $"Error: {error}";
            }
            catch (Exception ex)
            {
                Logger.Error($"Error executing command: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }
    }
}

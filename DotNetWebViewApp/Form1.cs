using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System.Text.Json.Serialization;
using System.Diagnostics;

namespace DotNetWebViewApp
{
    public partial class Form1 : Form
    {
        private WebView2 webView;
        private string indexFilePath;
        private string baseUrl;

        public Form1()
        {
            ShowSplashScreen(); // Show splash screen
            InitializeComponent();
            InitializeForm();
            this.WindowState = FormWindowState.Maximized; // Start in fullscreen
            this.MinimumSize = new Size(800, 600);        // Set minimum size
        }

        /// <summary>
        /// Displays a splash screen with a GIF animation.
        /// </summary>
        private void ShowSplashScreen()
        {
            string gifPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "browser", "splashScreen.gif");
            if (File.Exists(gifPath))
            {
                using var splashForm = new Form
                {
                    FormBorderStyle = FormBorderStyle.None,
                    StartPosition = FormStartPosition.CenterScreen,
                    BackgroundImage = Image.FromFile(gifPath),
                    BackgroundImageLayout = ImageLayout.Center,
                    Size = Image.FromFile(gifPath).Size
                };

                splashForm.Shown += (s, e) =>
                {
                    Task.Delay(3000).Wait(); // Display splash screen for 3 seconds
                    splashForm.Close();
                };

                splashForm.ShowDialog();
            }
        }

        /// <summary>
        /// Initializes the form by setting up the icon, file paths, WebView, and IPC handlers.
        /// </summary>
        private void InitializeForm()
        {
            SetFormIcon();
            InitializeFilePaths();
            InitializeWebView();
            RegisterIpcMainHandlers();
            Console.WriteLine("Form1 initialized.");
        }

        /// <summary>
        /// Sets the form's icon from the favicon file.
        /// </summary>
        private void SetFormIcon()
        {
            string faviconPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "browser", "favicon.ico");
            if (File.Exists(faviconPath))
            {
                this.Icon = new Icon(faviconPath);
            }
        }

        /// <summary>
        /// Initializes file paths for the WebView content.
        /// </summary>
        private void InitializeFilePaths()
        {
            indexFilePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "browser", "index.html");
            baseUrl = indexFilePath;
        }

        /// <summary>
        /// Initializes the WebView2 control and its environment.
        /// </summary>
        private async void InitializeWebView()
        {
            try
            {
                webView = new WebView2 { Dock = DockStyle.Fill };
                this.Controls.Add(webView);

                var environment = await CoreWebView2Environment.CreateAsync(
                    userDataFolder: null,
                    options: new CoreWebView2EnvironmentOptions("--disable-web-security --allow-file-access-from-files")
                );

                Console.WriteLine("Initializing WebView2...");
                await webView.EnsureCoreWebView2Async(environment);
                ConfigureWebViewSettings();
                SubscribeToWebViewEvents();

                if (webView.CoreWebView2 != null)
                {
                    await LoadWebViewContent();
                }
                else
                {
                    webView.CoreWebView2InitializationCompleted += async (sender, e) =>
                    {
                        if (e.IsSuccess)
                        {
                            await LoadWebViewContent();
                        }
                        else
                        {
                            Console.WriteLine("WebView2 initialization failed.");
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing WebView2: {ex.Message}");
            }
        }

        /// <summary>
        /// Configures WebView2 settings such as disabling context menus and DevTools.
        /// </summary>
        private void ConfigureWebViewSettings()
        {
            if (webView.CoreWebView2 == null) return;

            var settings = webView.CoreWebView2.Settings;
            settings.AreDefaultContextMenusEnabled = false;
            settings.AreBrowserAcceleratorKeysEnabled = false;
            settings.AreDevToolsEnabled = false;

            Console.WriteLine("WebView2 settings configured.");
        }

        /// <summary>
        /// Subscribes to WebView2 events such as message reception and document title changes.
        /// </summary>
        private void SubscribeToWebViewEvents()
        {
            if (webView.CoreWebView2 == null) return;

            webView.CoreWebView2.WebMessageReceived += HandleWebMessageReceived;
            webView.CoreWebView2.DocumentTitleChanged += (sender, e) =>
            {
                this.Text = webView.CoreWebView2.DocumentTitle ?? "DotNetWebViewApp";
            };

            this.KeyPreview = true;
            this.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.F12 || e.KeyCode == Keys.F5)
                {
                    e.Handled = true;
                }
            };

            webView.MouseDown += (s, args) =>
            {
                if (args.Button == MouseButtons.Left && args is HandledMouseEventArgs handledArgs)
                {
                    handledArgs.Handled = true;
                }
            };

            Console.WriteLine("WebView2 events subscribed.");
        }

        /// <summary>
        /// Loads the WebView content and injects the preload script.
        /// </summary>
        private async Task LoadWebViewContent()
        {
            try
            {
                string preloadScriptPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "browser", "preload.js");
                if (File.Exists(preloadScriptPath))
                {
                    string preloadScript = await File.ReadAllTextAsync(preloadScriptPath);
                    await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(preloadScript);
                    Console.WriteLine("Preload script injected.");
                }
                else
                {
                    Console.WriteLine($"Preload script not found at path: {preloadScriptPath}");
                }

                webView.CoreWebView2.Navigate(baseUrl);
                Console.WriteLine("WebView content loaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading WebView content: {ex.Message}");
            }
        }

        /// <summary>
        /// Registers IPC handlers using IpcMain.
        /// </summary>
        private void RegisterIpcMainHandlers()
        {
            IpcMain.Handle("version", async args => await Task.FromResult(Application.ProductVersion));
            IpcMain.Handle("status", async args => await Task.FromResult("Running"));
            IpcMain.Handle("platform", async args => await Task.FromResult(GetPlatformString(Environment.OSVersion.Platform)));
            IpcMain.Handle("openFolderDialog", async args => await Task.Run(() => OpenFolderDialog()));
            IpcMain.Handle("readFile", async args =>
            {
                if (args.Length > 0 && args[0] is string filePath)
                {
                    return await Task.Run(() => File.ReadAllText(filePath));
                }
                return string.Empty;
            });
            IpcMain.Handle("saveFile", async args =>
            {
                if (args.Length >= 2 && args[0] is string filePath && args[1] is string content)
                {
                    await Task.Run(() => File.WriteAllText(filePath, content));
                    return "File saved successfully";
                }
                return "Invalid arguments";
            });
            IpcMain.Handle("readdir", async args =>
            {
                if (args.Length > 0 && args[0] is string directoryPath)
                {
                    var files = await Task.Run(() => Directory.GetFiles(directoryPath));
                    return System.Text.Json.JsonSerializer.Serialize(files);
                }
                return string.Empty;
            });
            IpcMain.Handle("showMessageBox", async args =>
            {
                return await Task.Run(() =>
                {
                    if (args.Length > 0 && args[0] is string message)
                    {
                        MessageBox.Show(message);
                        return "Message box shown";
                    }
                    return "Invalid arguments";
                });
            });
            IpcMain.Handle("getToken", async args => await Task.FromResult("token"));
            IpcMain.Handle("getAuthProfile", async args =>
                await Task.Run(() => System.Text.Json.JsonSerializer.Serialize(new { name = "User", email = "user@example.com" })));
            IpcMain.Handle("closeMainWindow", async args =>
            {
                await Task.Run(() => this.Invoke((Action)(() => this.Close())));
                return "Main window closed";
            });
            IpcMain.Handle("runCommand", async args =>
            {
                return await Task.Run(() =>
                {
                    if (args.Length > 0 && args[0] is string command)
                    {
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "cmd.exe",
                                Arguments = $"/C {command}",
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                            }
                        };
                        process.Start();
                        string result = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();
                        return result;
                    }
                    return string.Empty;
                });
            });
            IpcMain.Handle("getUserHost", async args =>
                await Task.Run(() => System.Text.Json.JsonSerializer.Serialize(new { username = Environment.UserName, hostname = Environment.MachineName })));

            Console.WriteLine("IpcMain handlers registered.");
        }

        /// <summary>
        /// Handles messages received from the WebView and processes them using IpcMain.
        /// </summary>
        private void HandleWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var message = e.TryGetWebMessageAsString();
                var messageObject = System.Text.Json.JsonSerializer.Deserialize<Message>(message);

                if (messageObject != null)
                {
                    Console.WriteLine($"Message received: Channel = {messageObject.Channel}, Args = {string.Join(", ", messageObject.Args)}");

                    // Emit the event using IpcMain
                    IpcMain.Emit(messageObject.Channel, messageObject.Args);

                    // Handle invokeable IPCs
                    if (IpcMain.HasHandler(messageObject.Channel))
                    {
                        var response = IpcMain.Invoke(messageObject.Channel, messageObject.Args).Result;
                        var responseString = System.Text.Json.JsonSerializer.Serialize(new { channel = messageObject.Channel, result = response });
                        webView.CoreWebView2.PostWebMessageAsString(responseString);
                        Console.WriteLine($"Response sent for channel: {messageObject.Channel}");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to deserialize message: messageObject is null.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                var errorResponse = new { channel = "error", error = ex.Message };
                webView.CoreWebView2.PostWebMessageAsString(System.Text.Json.JsonSerializer.Serialize(errorResponse));
            }
        }

        /// <summary>
        /// Opens a folder dialog and returns the selected path.
        /// </summary>
        private string OpenFolderDialog()
        {
            using var dialog = new FolderBrowserDialog();
            return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : string.Empty;
        }

        /// <summary>
        /// Maps PlatformID to a user-friendly string.
        /// </summary>
        private string GetPlatformString(PlatformID platformId) =>
            platformId switch
            {
                PlatformID.Win32NT => "win32",
                PlatformID.Unix => "linux",
                PlatformID.MacOSX => "darwin",
                _ => "unknown"
            };

        /// <summary>
        /// Represents a message structure for communication.
        /// </summary>
        private class Message
        {
            [JsonPropertyName("channel")]
            public string Channel { get; set; } = string.Empty;

            [JsonPropertyName("args")]
            public string[] Args { get; set; } = Array.Empty<string>();
        }
    }
}

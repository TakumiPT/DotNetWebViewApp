using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System.Text.Json.Serialization;

namespace DotNetWebViewApp
{
    public partial class Form1 : Form
    {
        private WebView2 webView;
        private string indexFilePath;
        private string baseUrl;
        private readonly bool isDev = true; // Debug mode flag

        private const string BrowserFolder = "wwwroot\\browser";
        private const string SplashScreenFile = "splashScreen.gif";
        private const string FaviconFile = "favicon.ico";
        private const string PreloadScriptFile = "preload.js";

        public Form1()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            ShowSplashScreen();
            ConfigureFormProperties();
            SetFormIcon();
            InitializeFilePaths();
            InitializeWebView();
            RegisterIpcMainHandlers();
        }

        private void ConfigureFormProperties()
        {
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(800, 600);
        }

        private void ShowSplashScreen()
        {
            string gifPath = Path.Combine(AppContext.BaseDirectory, BrowserFolder, SplashScreenFile);
            if (!File.Exists(gifPath)) return;

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
                Task.Delay(3000).Wait();
                splashForm.Close();
            };

            splashForm.ShowDialog();
        }

        private void SetFormIcon()
        {
            string faviconPath = Path.Combine(AppContext.BaseDirectory, BrowserFolder, FaviconFile);
            if (File.Exists(faviconPath))
            {
                this.Icon = new Icon(faviconPath);
            }
        }

        private void InitializeFilePaths()
        {
            indexFilePath = Path.Combine(AppContext.BaseDirectory, BrowserFolder, "index.html");
            baseUrl = indexFilePath;
        }

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

                webView.CoreWebView2InitializationCompleted += (sender, e) =>
                {
                    if (e.IsSuccess)
                    {
                        ConfigureWebViewSettings();
                        SubscribeToWebViewEvents();
                    }
                    else
                    {
                        Console.WriteLine("WebView2 initialization failed.");
                    }
                };

                await webView.EnsureCoreWebView2Async(environment);

                if (webView.CoreWebView2 != null)
                {
                    ConfigureWebViewSettings();
                    SubscribeToWebViewEvents();
                    await LoadWebViewContent();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing WebView2: {ex.Message}");
            }
        }

        private void ConfigureWebViewSettings()
        {
            if (webView.CoreWebView2 == null) return;

            var settings = webView.CoreWebView2.Settings;
            settings.AreDefaultContextMenusEnabled = isDev;
            settings.AreBrowserAcceleratorKeysEnabled = isDev;
            settings.AreDevToolsEnabled = isDev;

            Console.WriteLine($"WebView2 settings configured. Debug mode: {isDev}, DevTools enabled: {settings.AreDevToolsEnabled}");
        }

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
                if (isDev && (e.KeyCode == Keys.F5 || e.KeyCode == Keys.F12))
                {
                    if (e.KeyCode == Keys.F5)
                    {
                        webView.CoreWebView2.Reload();
                        Console.WriteLine("WebView content refreshed.");
                    }
                    else if (e.KeyCode == Keys.F12)
                    {
                        webView.CoreWebView2.OpenDevToolsWindow();
                        Console.WriteLine("DevTools window opened.");
                    }
                    e.Handled = true;
                }
            };

            Console.WriteLine("WebView2 events subscribed.");
        }

        private async Task LoadWebViewContent()
        {
            try
            {
                string preloadScriptPath = Path.Combine(AppContext.BaseDirectory, BrowserFolder, PreloadScriptFile);
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

        private void RegisterIpcMainHandlers()
        {
            IpcMainSingleton.Instance.RegisterHandlers();
        }

        private void HandleWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var message = e.TryGetWebMessageAsString();
                Console.WriteLine($"WebMessageReceived: {message}");

                var messageObject = System.Text.Json.JsonSerializer.Deserialize<Message>(message);

                if (messageObject != null)
                {
                    Console.WriteLine($"Message received: Channel = {messageObject.Channel}, Args = {string.Join(", ", messageObject.Args)}");

                    // Emit the event without assigning its result
                    IpcMainSingleton.Instance.Emit(messageObject.Channel, messageObject.Args);

                    // Optionally send a response back to the WebView
                    var responseString = System.Text.Json.JsonSerializer.Serialize(new { channel = messageObject.Channel, result = "Handled" });
                    webView.CoreWebView2.PostWebMessageAsString(responseString);
                    Console.WriteLine($"Response sent: {responseString}");
                }
                else
                {
                    Console.WriteLine("Failed to deserialize message.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }

        private class Message
        {
            [JsonPropertyName("channel")]
            public string Channel { get; set; } = string.Empty;

            [JsonPropertyName("args")]
            public string[] Args { get; set; } = Array.Empty<string>();
        }
    }
}

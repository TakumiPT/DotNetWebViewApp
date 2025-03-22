using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System.Text.Json.Serialization;

namespace DotNetWebViewApp
{
    public class WebViewService : IWebViewService
    {
        public const string BrowserFolder = "wwwroot\\browser";
        public const string SplashScreenFile = "splashScreen.gif";
        public const string FaviconFile = "favicon.ico";
        public const string PreloadScriptFile = "preload.js";

        private readonly Form parentForm;
        private readonly bool isDev; // Debug mode flag
        private WebView2 webView;
        private WebViewEventHandler eventHandler;
        private string indexFilePath;
        private string baseUrl;

        public WebViewService(Form parentForm, bool isDev)
        {
            this.parentForm = parentForm;
            this.isDev = isDev; // Store isDev flag
        }

        public async void Initialize()
        {
            try
            {
                webView = await WebViewFactory.CreateWebViewAsync(parentForm);
                InitializeFilePaths();
                ConfigureSettings();
                eventHandler = new WebViewEventHandler(webView, parentForm);
                eventHandler.Subscribe();
                await LoadContent();
            }
            catch (Exception ex)
            {
                Logger.Error("Error during WebViewService initialization", ex);
            }
        }

        private void InitializeFilePaths()
        {
            indexFilePath = Path.Combine(AppContext.BaseDirectory, BrowserFolder, "index.html");
            baseUrl = $"file:///{indexFilePath.Replace("\\", "/")}";
            Console.WriteLine($"Index file path resolved to: {indexFilePath}");
            Console.WriteLine($"Base URL set to: {baseUrl}");
        }

        private void ConfigureSettings()
        {
            if (webView.CoreWebView2 == null) return;

            var builder = new WebViewSettingsBuilder(webView.CoreWebView2.Settings)
                .EnableContextMenus(isDev)
                .EnableAcceleratorKeys(isDev)
                .EnableDevTools(isDev);

            builder.Build();
        }

        private async Task LoadContent()
        {
            try
            {
                string preloadScriptPath = Path.Combine(AppContext.BaseDirectory, BrowserFolder, PreloadScriptFile);
                Console.WriteLine($"Preload script path resolved to: {preloadScriptPath}");

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

                if (File.Exists(indexFilePath))
                {
                    webView.CoreWebView2.Navigate(baseUrl);
                    Console.WriteLine("WebView content loaded.");
                }
                else
                {
                    Console.WriteLine($"Index file not found at path: {indexFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading WebView content: {ex.Message}");
            }
        }
    }
}

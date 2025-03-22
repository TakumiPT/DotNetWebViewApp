using Microsoft.Web.WebView2.WinForms;

namespace DotNetWebViewApp
{
    public partial class Form1 : Form
    {
        private readonly WebViewService webViewService;
        private readonly bool isDev = false; // Debug mode flag
        private readonly EventAggregator eventAggregator = new EventAggregator(); // Instance of EventAggregator

        public Form1()
        {
            InitializeComponent();
            webViewService = new WebViewService(this, isDev); // Pass isDev to WebViewService
            InitializeForm();
        }

        private void InitializeForm()
        {
            IpcHandlerFactory.RegisterHandlers(); // Ensure handlers are registered before WebView initialization
            ShowSplashScreen();
            ConfigureFormProperties();
            SetFormIcon();
            webViewService.Initialize(); // Delegate WebView initialization
            eventAggregator.Publish("ApplicationInitialized"); // Notify subscribers
        }

        private void ConfigureFormProperties()
        {
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(800, 600);
        }

        private void ShowSplashScreen()
        {
            string gifPath = Path.Combine(AppContext.BaseDirectory, ConfigurationManager.BrowserFolder, ConfigurationManager.SplashScreenFile);
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
            string faviconPath = Path.Combine(AppContext.BaseDirectory, ConfigurationManager.BrowserFolder, ConfigurationManager.FaviconFile);
            if (File.Exists(faviconPath))
            {
                this.Icon = new Icon(faviconPath);
            }
        }
    }
}

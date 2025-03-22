using Microsoft.Web.WebView2.Core;

namespace DotNetWebViewApp
{
    public class WebViewSettingsBuilder
    {
        private readonly CoreWebView2Settings settings;

        public WebViewSettingsBuilder(CoreWebView2Settings settings)
        {
            this.settings = settings;
        }

        public WebViewSettingsBuilder EnableContextMenus(bool enable)
        {
            settings.AreDefaultContextMenusEnabled = enable;
            return this;
        }

        public WebViewSettingsBuilder EnableAcceleratorKeys(bool enable)
        {
            settings.AreBrowserAcceleratorKeysEnabled = enable;
            return this;
        }

        public WebViewSettingsBuilder EnableDevTools(bool enable)
        {
            settings.AreDevToolsEnabled = enable;
            return this;
        }

        public void Build()
        {
            Console.WriteLine("WebView2 settings configured.");
        }
    }
}

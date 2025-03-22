using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace DotNetWebViewApp
{
    public static class WebViewFactory
    {
        public static async Task<WebView2> CreateWebViewAsync(Form parentForm)
        {
            var webView = new WebView2 { Dock = DockStyle.Fill };
            parentForm.Controls.Add(webView);

            var environment = await CoreWebView2Environment.CreateAsync(
                userDataFolder: null,
                options: new CoreWebView2EnvironmentOptions("--disable-web-security --allow-file-access-from-files")
            );

            await webView.EnsureCoreWebView2Async(environment);
            return webView;
        }
    }
}

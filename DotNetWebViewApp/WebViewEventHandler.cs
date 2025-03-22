using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Text.Json.Serialization;

namespace DotNetWebViewApp
{
    public class WebViewEventHandler : IWebViewEventHandler
    {
        private readonly WebView2 webView;
        private readonly Form parentForm;

        public WebViewEventHandler(WebView2 webView, Form parentForm)
        {
            this.webView = webView;
            this.parentForm = parentForm;
        }

        public void Subscribe()
        {
            webView.CoreWebView2.WebMessageReceived += HandleWebMessageReceived;
            webView.CoreWebView2.DocumentTitleChanged += (sender, e) =>
            {
                parentForm.Text = webView.CoreWebView2.DocumentTitle ?? "DotNetWebViewApp";
            };

            Logger.Info("WebView2 events subscribed.");
        }

        private async void HandleWebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                var message = e.TryGetWebMessageAsString();
                Logger.Info($"WebMessageReceived: {message}");

                var messageObject = System.Text.Json.JsonSerializer.Deserialize<Message>(message);

                if (messageObject != null)
                {
                    Logger.Info($"Message received: Channel = {messageObject.Channel}, Args = {string.Join(", ", messageObject.Args)}");

                    // Emit the event and handle invokeable IPCs
                    IpcMain.Emit(messageObject.Channel, messageObject.Args);

                    if (IpcMain.HasHandler(messageObject.Channel))
                    {
                        var result = await IpcMain.Invoke(messageObject.Channel, messageObject.Args);
                        Logger.Info($"Handler result for channel '{messageObject.Channel}': {result}");

                        // Send the result back to the WebView
                        var response = new
                        {
                            channel = messageObject.Channel,
                            result
                        };
                        var responseString = System.Text.Json.JsonSerializer.Serialize(response);
                        webView.CoreWebView2.PostWebMessageAsString(responseString);
                        Logger.Info($"Response sent to WebView: {responseString}");
                    }
                    else
                    {
                        Logger.Warning($"No handler found for channel: {messageObject.Channel}");
                    }
                }
                else
                {
                    Logger.Warning("Failed to deserialize message.");
                }
            }
            catch (Exception ex)
            {
                HandleError(ex, "WebMessageReceived");
            }
        }

        private void HandleError(Exception ex, string context)
        {
            GlobalErrorHandler.Handle(ex, context);

            // Send an error response back to the WebView
            var errorResponse = new
            {
                channel = "error",
                error = ex.Message
            };
            webView.CoreWebView2.PostWebMessageAsString(System.Text.Json.JsonSerializer.Serialize(errorResponse));
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

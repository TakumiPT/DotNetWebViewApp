using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using System;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DotNetWebViewApp
{
    public partial class Form1 : Form
    {
        // WebView2 control to host the Angular application
        private WebView2 webView;
        private Dictionary<string, Func<string[], string>> channelHandlers;

        public Form1()
        {
            InitializeComponent();
            InitializeWebView();
            InitializeChannelHandlers();
            Console.WriteLine("Form1 constructor executed.");
        }

        // Initialize the WebView2 control
        private void InitializeWebView()
        {
            webView = new WebView2
            {
                Dock = DockStyle.Fill // Fill the entire form
            };
            this.Controls.Add(webView);
            webView.Source = new Uri("http://localhost:4200"); // Set the source to the Angular app
            // Subscribe to the CoreWebView2InitializationCompleted event
            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
            Console.WriteLine("WebView2 initialization started.");
        }

        // Initialize the channel handlers
        private void InitializeChannelHandlers()
        {
            channelHandlers = new Dictionary<string, Func<string[], string>>
            {
                { "version", HandleVersionRequest },
                { "status", HandleStatusRequest },
                { "platform", HandlePlatformRequest },
                { "openFolderDialog", HandleOpenFolderDialogRequest },
                { "readFile", HandleReadFileRequest },
                { "saveFile", HandleSaveFileRequest },
                { "readdir", HandleReadDirRequest },
                { "showMessageBox", HandleShowMessageBoxRequest },
                { "getToken", HandleGetTokenRequest },
                { "getAuthProfile", HandleGetAuthProfileRequest },
                { "closeMainWindow", HandleCloseMainWindowRequest },
                { "runCommand", HandleRunCommandRequest },
                { "getUserHost", HandleGetUserHostRequest }
            };
        }

        // Event handler for WebView2 initialization completion
        private async void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            Console.WriteLine("WebView2 initialization completed.");
            if (e.IsSuccess)
            {
                Console.WriteLine("WebView2 initialization successful.");
                // Add an event listener for messages from the Angular app
                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                Console.WriteLine("WebMessageReceived event subscribed.");
                // Inject the preload script
                Console.WriteLine(AppContext.BaseDirectory);
                string preloadScriptPath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "preload.js");
                if (File.Exists(preloadScriptPath))
                {
                    string preloadScript = await System.IO.File.ReadAllTextAsync(preloadScriptPath);
                    await webView.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(preloadScript);
                    Console.WriteLine("Preload script injected.");
                }
                else
                {
                    Console.WriteLine($"Preload script not found at path: {preloadScriptPath}");
                }
                // Add a delay to ensure the script is loaded before the Angular application starts
                await Task.Delay(1000);
                webView.CoreWebView2.Navigate("http://localhost:4200");
            }
            else
            {
                Console.WriteLine("WebView2 initialization failed.");
            }
        }

        // Event handler for messages received from the Angular app
        private void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            Console.WriteLine("WebMessageReceived event triggered.");
            try
            {
                Console.WriteLine($"CoreWebView2WebMessageReceivedEventArgs {e}");
                var message = e.TryGetWebMessageAsString();
                Console.WriteLine($"Message received: {message}");
                var messageObject = System.Text.Json.JsonSerializer.Deserialize<Message>(message);
                if (messageObject != null)
                {
                    Console.WriteLine($"Message deserialized: Channel = {messageObject.Channel}, Args = {string.Join(", ", messageObject.Args)}");
                    if (channelHandlers.TryGetValue(messageObject.Channel, out var handler))
                    {
                        var response = handler(messageObject.Args);
                        var responseString = System.Text.Json.JsonSerializer.Serialize(new { channel = messageObject.Channel, result = response });
                        Console.WriteLine($"Response string: {responseString}");
                        try
                        {
                            webView.CoreWebView2.PostWebMessageAsString(responseString);
                            Console.WriteLine($"Response sent for channel: {messageObject.Channel}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception during PostWebMessageAsString: {ex.Message}");
                            var errorResponse = new { channel = messageObject.Channel, error = ex.Message };
                            webView.CoreWebView2.PostWebMessageAsString(System.Text.Json.JsonSerializer.Serialize(errorResponse));
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No handler found for channel: {messageObject.Channel}");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to deserialize message: messageObject is null.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during message processing: {ex.Message}");
                var errorResponse = new { channel = "error", error = ex.Message };
                webView.CoreWebView2.PostWebMessageAsString(System.Text.Json.JsonSerializer.Serialize(errorResponse));
            }
        }

        // Handlers for different channels
        private string HandleVersionRequest(string[] args)
        {
            return Application.ProductVersion;
        }

        private string HandleStatusRequest(string[] args)
        {
            return "Running";
        }

        private string HandlePlatformRequest(string[] args)
        {
            return GetPlatformString(Environment.OSVersion.Platform);
        }

        private string HandleOpenFolderDialogRequest(string[] args)
        {
            return OpenFolderDialog();
        }

        private string HandleReadFileRequest(string[] args)
        {
            if (args.Length == 0) return string.Empty;
            return File.ReadAllText(args[0]);
        }

        private string HandleSaveFileRequest(string[] args)
        {
            if (args.Length < 2) return string.Empty;
            File.WriteAllText(args[0], args[1]);
            return "File saved successfully";
        }

        private string HandleReadDirRequest(string[] args)
        {
            if (args.Length == 0) return string.Empty;
            var files = Directory.GetFiles(args[0]);
            return System.Text.Json.JsonSerializer.Serialize(files);
        }

        private string HandleShowMessageBoxRequest(string[] args)
        {
            if (args.Length == 0) return string.Empty;
            MessageBox.Show(args[0]);
            return "Message box shown";
        }

        private string HandleGetTokenRequest(string[] args)
        {
            // Implement your logic to get the token
            return "token";
        }

        private string HandleGetAuthProfileRequest(string[] args)
        {
            // Implement your logic to get the auth profile
            return System.Text.Json.JsonSerializer.Serialize(new { name = "User", email = "user@example.com" });
        }

        private string HandleCloseMainWindowRequest(string[] args)
        {
            this.Close();
            return "Main window closed";
        }

        private string HandleRunCommandRequest(string[] args)
        {
            if (args.Length == 0) return string.Empty;
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C {args[0]}",
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

        private string HandleGetUserHostRequest(string[] args)
        {
            return System.Text.Json.JsonSerializer.Serialize(new { username = Environment.UserName, hostname = Environment.MachineName });
        }

        // Method to open a folder dialog and return the selected path
        private string OpenFolderDialog()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
            }
            return string.Empty;
        }

        // Event handler for form load
        private void Form1_Load(object? sender, EventArgs e)
        {
            // Add a test button to send a message to the Angular app
            Button testButton = new Button
            {
                Text = "Send Message",
                Dock = DockStyle.Top
            };
            testButton.Click += TestButton_Click;
            this.Controls.Add(testButton);
            Console.WriteLine("Form loaded and test button added.");
        }

        // Event handler for the test button click
        private void TestButton_Click(object? sender, EventArgs e)
        {
            // Send a message to the Angular app
            webView.CoreWebView2.PostWebMessageAsString("Hello from .NET!");
            Console.WriteLine("Test message sent to Angular app.");
        }

        // Class to represent the message structure
        private class Message
        {
            [JsonPropertyName("channel")]
            public string Channel { get; set; } = string.Empty;

            [JsonPropertyName("args")]
            public string[] Args { get; set; } = Array.Empty<string>();
        }

        // Method to map PlatformID to user-friendly string
        private string GetPlatformString(PlatformID platformId)
        {
            return platformId switch
            {
                PlatformID.Win32NT => "win32",
                PlatformID.Unix => "linux",
                PlatformID.MacOSX => "darwin",
                _ => "unknown"
            };
        }
    }
}

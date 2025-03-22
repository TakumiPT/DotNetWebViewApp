using System;

namespace DotNetWebViewApp.Services
{
    /// <summary>
    /// Service to handle process-related operations.
    /// </summary>
    public class ProcessService
    {
        /// <summary>
        /// Gets the current platform of the process.
        /// </summary>
        public string GetPlatform() => Environment.OSVersion.Platform.ToString();
    }
}

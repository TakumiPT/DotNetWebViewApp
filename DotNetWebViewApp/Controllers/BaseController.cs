using DotNetWebViewApp;

namespace DotNetWebViewApp.Controllers
{
    /// <summary>
    /// Abstract base class for controllers that handle IPC communication.
    /// </summary>
    public abstract class BaseController
    {
        /// <summary>
        /// Gets the IPC channel name.
        /// </summary>
        public abstract string Channel { get; }

        /// <summary>
        /// Initializes IPC handlers for the controller.
        /// </summary>
        public abstract void InitializeIpcHandlers();

        /// <summary>
        /// Registers an IPC handler for a specific endpoint.
        /// </summary>
        protected void RegisterIpcHandler(string endpoint, Func<object[], Task<object>> handler)
        {
            IpcMain.Handle($"{Channel}/{endpoint}", handler);
        }
    }
}

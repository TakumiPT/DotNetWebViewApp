using DotNetWebViewApp.Services;

namespace DotNetWebViewApp.Controllers
{
    /// <summary>
    /// Controller for handling process-related IPC communications.
    /// </summary>
    public class ProcessController : BaseController
    {
        private readonly ProcessService processService;

        public ProcessController(ProcessService processService)
        {
            this.processService = processService;
            InitializeIpcHandlers();
        }

        public override string Channel => "process";

        public override void InitializeIpcHandlers()
        {
            RegisterIpcHandler("platform", async args => await Task.FromResult(processService.GetPlatform()));
        }
    }
}

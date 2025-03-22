using DotNetWebViewApp.Services;

namespace DotNetWebViewApp.Controllers
{
    /// <summary>
    /// Controller for handling application-related IPC communications.
    /// </summary>
    public class AppController : BaseController
    {
        private readonly AppService appService;

        public AppController(AppService appService)
        {
            this.appService = appService;
            InitializeIpcHandlers();
        }

        public override string Channel => "app";

        public override void InitializeIpcHandlers()
        {
            RegisterIpcHandler("version", async args => await Task.FromResult(appService.GetVersion()));
            RegisterIpcHandler("name", async args => await Task.FromResult(appService.GetName()));
            RegisterIpcHandler("path", async args => await Task.FromResult(appService.GetAppPath()));
            RegisterIpcHandler("quit", async args =>
            {
                appService.Quit();
                return await Task.FromResult("Application quitting...");
            });
            RegisterIpcHandler("languages", async args => await Task.FromResult(appService.GetPreferredSystemLanguages()));
            RegisterIpcHandler("locale", async args => await Task.FromResult(appService.GetLocale()));
            RegisterIpcHandler("localeCountryCode", async args => await Task.FromResult(appService.GetLocaleCountryCode()));
        }
    }
}

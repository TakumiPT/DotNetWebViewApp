namespace DotNetWebViewApp
{
    public static class GlobalErrorHandler
    {
        public static void Handle(Exception ex, string context)
        {
            Logger.Error($"Error in {context}", ex);
        }

        public static void RegisterGlobalExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                {
                    Handle(ex, "UnhandledException");
                }
            };

            TaskScheduler.UnobservedTaskException += (sender, args) =>
            {
                Handle(args.Exception, "UnobservedTaskException");
                args.SetObserved();
            };
        }
    }
}

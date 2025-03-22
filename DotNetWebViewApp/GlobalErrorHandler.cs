namespace DotNetWebViewApp
{
    public static class GlobalErrorHandler
    {
        public static void Handle(Exception ex, string context)
        {
            Logger.Error($"Error in {context}", ex);
        }
    }
}

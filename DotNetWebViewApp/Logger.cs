namespace DotNetWebViewApp
{
    public static class Logger
    {
        private static string GetTimestamp()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public static void Debug(string message)
        {
            Console.WriteLine($"[{GetTimestamp()}] [DEBUG] {message}");
        }

        public static void Info(string message)
        {
            Console.WriteLine($"[{GetTimestamp()}] [INFO] {message}");
        }

        public static void Warning(string message)
        {
            Console.WriteLine($"[{GetTimestamp()}] [WARNING] {message}");
        }

        public static void Error(string message, Exception ex = null)
        {
            Console.WriteLine($"[{GetTimestamp()}] [ERROR] {message}");
            if (ex != null)
            {
                Console.WriteLine($"[{GetTimestamp()}] [ERROR] Exception: {ex.Message}");
            }
        }
    }
}

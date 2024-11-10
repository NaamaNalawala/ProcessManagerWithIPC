using System;

namespace OldConsoleApp.Services
{
    // Simple logger class for error logging
    public static class Logger
    {
        private static readonly object _consoleLock = new object();

        public static void Log(string message)
        {
            lock (_consoleLock)
            {
                Console.WriteLine($"[LOG] {DateTime.Now:HH:mm:ss.fff}: {message}");
            }
        }
    }
}

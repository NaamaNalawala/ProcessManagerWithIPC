using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace NewConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConsoleHandler consoleHandler = new ConsoleHandler(new NamedPipeMessageSender("GameBoardPipe"));

            string uniqueId = args.Length > 0 ? args[0] : Guid.NewGuid().ToString();
            Console.WriteLine($"New Console Started with ID: {uniqueId}");

            await consoleHandler.StartAsync(uniqueId);
        }
    }

    // Interface for message sending
    public interface IMessageSender : IAsyncDisposable
    {
        Task SendMessageAsync(string consoleId, string message);
    }

    // Named pipe message sender implementation
    public class NamedPipeMessageSender : IMessageSender
    {
        private readonly string _pipeName;

        public NamedPipeMessageSender(string pipeName)
        {
            _pipeName = pipeName;
        }

        public async Task SendMessageAsync(string consoleId, string message)
        {
            try
            {
                using var client = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out);
                await client.ConnectAsync();
                using var writer = new StreamWriter(client) { AutoFlush = true };
                await writer.WriteLineAsync($"{consoleId}:{message}");
            }
            catch (Exception ex)
            {
                Logger.Log($"Error sending message: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            // Clean up resources if necessary
            await Task.CompletedTask;
        }
    }

    // Console handler class
    public class ConsoleHandler : IConsoleHandler
    {
        private readonly IMessageSender _messageSender;

        public ConsoleHandler(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public async Task StartAsync(string uniqueId)
        {
            while (true)
            {
                Console.Write("Enter a score: ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input cannot be empty. Please try again.");
                    continue;
                }

                await _messageSender.SendMessageAsync(uniqueId, input);
            }
        }
    }

    // Interface for console handling
    public interface IConsoleHandler
    {
        Task StartAsync(string uniqueId);
    }

    // Simple logger class for error logging
    public static class Logger
    {
        public static void Log(string message)
        {
            Console.WriteLine($"[LOG] {DateTime.Now}: {message}");
        }
    }
}

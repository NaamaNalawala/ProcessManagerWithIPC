using System;
using System.Threading.Tasks;
using NewConsoleApp.Interfaces;
using NewConsoleApp.Services;

namespace NewConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IMessageSender messageSender = new NamedPipeMessageSender("GameBoardPipe");
            IConsoleHandler consoleHandler = new ConsoleHandler(messageSender);

            string uniqueId = args.Length > 0 ? args[0] : Guid.NewGuid().ToString();
            Console.WriteLine($"New Console Started with ID: {uniqueId}");

            await consoleHandler.StartAsync(uniqueId);
        }
    }
}

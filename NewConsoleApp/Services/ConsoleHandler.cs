using System;
using System.Threading.Tasks;
using NewConsoleApp.Interfaces;

namespace NewConsoleApp.Services
{
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
}

using OldConsoleApp.Interfaces;
using OldConsoleApp.Services;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace OldConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a Console ID.");
                return;
            }

            string consoleId = args[0];
            Console.WriteLine($"Old Console started with ID: {consoleId}");

            IScoreData scoreData = new ScoreData();
            IPipeServer pipeServer = new NamedPipeServer(consoleId, scoreData);
            IUserInputHandler inputHandler = new UserInputHandler(scoreData);

            // Start the named pipe server and user input handling
            _ = Task.Run(() => pipeServer.StartAsync());
            await inputHandler.StartAsync();
        }
    }

}
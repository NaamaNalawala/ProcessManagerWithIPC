using System;
using System.Collections.Generic;
using System.Threading;
using GameBoard.Interfaces;
using GameBoard.Models;
using GameBoard.Services;

namespace GameBoard
{
    class Program
    {
        static void Main(string[] args)
        {
            int numberOfNewConsoles = GetConsoleCount("NewConsoles");
            int numberOfOldConsoles = GetConsoleCount("OldConsoles");

            var consoles = new List<IConsoleProcess>();
            var oldConsoleProcesses = new List<OldConsoleProcess>();
            var consoleIds = new List<string>();

            // Start New Consoles
            for (int i = 1; i <= numberOfNewConsoles; i++)
            {
                string consoleId = $"Console_{i}";
                var console = ConsoleProcessFactory.CreateConsoleProcess("NewConsole", consoleId);
                consoles.Add(console);
                consoleIds.Add(consoleId);
            }

            // Start Old Consoles
            for (int i = 1; i <= numberOfOldConsoles; i++)
            {
                string consoleId = $"OldConsole_{i}";
                var console = (OldConsoleProcess)ConsoleProcessFactory.CreateConsoleProcess("OldConsole", consoleId);
                consoles.Add(console);
                oldConsoleProcesses.Add(console);
                consoleIds.Add(consoleId);
            }

            // Start all consoles
            foreach (var console in consoles)
            {
                console.StartAsync().Wait();
                Console.WriteLine($"Started console with ID: {console.ConsoleId}");
            }

            Console.WriteLine($"Successfully started {numberOfNewConsoles} NewConsoles and {numberOfOldConsoles} OldConsoles.");

            // Initialize GameBoard
            var gameBoard = new GameBoard.Services.MainGameBoard();
            gameBoard.InitializeConsoleData(consoleIds);

            var cancellationTokenSource = new CancellationTokenSource();

            // Start Named Pipe Listener
            var namedPipeListener = new NamedPipeListener("GameBoardPipe", gameBoard);
            _ = namedPipeListener.StartListeningAsync(cancellationTokenSource.Token);

            // Start Old Console Poller
            var oldConsolePoller = new OldConsolePoller(oldConsoleProcesses, gameBoard);
            oldConsolePoller.StartPolling();

            // Display GameBoard
            gameBoard.Display();
        }

        static int GetConsoleCount(string consoleType)
        {
            int numberOfConsoles;
            do
            {
                Console.Write($"Enter the number of {consoleType} to start (0-100): ");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out numberOfConsoles) || numberOfConsoles < 0 || numberOfConsoles > 100)
                {
                    Console.WriteLine($"Invalid Input for {consoleType}, value must be between 0 and 100");
                }
                else
                {
                    break;
                }
            } while (true);

            return numberOfConsoles;
        }
    }
}

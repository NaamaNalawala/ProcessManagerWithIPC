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

    // Interface for the named pipe server
    public interface IPipeServer
    {
        Task StartAsync();
    }

    // Named pipe server implementation
    public class NamedPipeServer : IPipeServer
    {
        private readonly string _pipeName;
        private readonly IScoreData _scoreData;

        public NamedPipeServer(string pipeName, IScoreData scoreData)
        {
            _pipeName = pipeName;
            _scoreData = scoreData;
        }

        public async Task StartAsync()
        {
            while (true)
            {
                try
                {
                    using var server = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                    Logger.Log($"Waiting for connection: {_pipeName}");

                    await server.WaitForConnectionAsync();
                    Logger.Log($"Connection established: {_pipeName}");

                    using var writer = new StreamWriter(server) { AutoFlush = true };

                    // Write the current score data to the stream
                    string dataToWrite = _scoreData.CurrentData;
                    await writer.WriteLineAsync(dataToWrite);
                    Logger.Log($"Wrote: {dataToWrite}");

                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Error in NamedPipeServer: {ex.Message}");
                }
            }
        }
    }

    // Interface for score data management
    public interface IScoreData
    {
        string CurrentData { get; set; }
    }

    // Score data implementation
    public class ScoreData : IScoreData
    {
        public string CurrentData { get; set; } = "0";
    }

    // Interface for handling user input
    public interface IUserInputHandler
    {
        Task StartAsync();
    }

    // User input handler implementation
    public class UserInputHandler : IUserInputHandler
    {
        private readonly IScoreData _scoreData;

        public UserInputHandler(IScoreData scoreData)
        {
            _scoreData = scoreData;
        }

        public async Task StartAsync()
        {
            while (true)
            {
                Console.Write("Enter a score: ");
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    _scoreData.CurrentData = input;
                    Logger.Log($"Updated score: {_scoreData.CurrentData}");
                }

                await Task.Delay(100); // Small delay to prevent high CPU usage
            }
        }
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

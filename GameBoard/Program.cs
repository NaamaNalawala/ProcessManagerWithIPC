using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace GameBoard
{
    public interface IConsoleProcess
    {
        string ConsoleId { get; }
        Task StartAsync();
    }

    public interface IConsoleDataListener
    {
        void OnDataReceived(string consoleId, string data);
    }

    public class NewConsoleProcess : IConsoleProcess
    {
        public string ConsoleId { get; }

        public NewConsoleProcess(string consoleId)
        {
            ConsoleId = consoleId;
        }

        public async Task StartAsync()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "NewConsoleApp.exe",
                Arguments = ConsoleId,
                UseShellExecute = true,
                CreateNoWindow = false
            };

            Process.Start(psi);
            await Task.CompletedTask;
        }
    }

    public class OldConsoleProcess : IConsoleProcess
    {
        public string ConsoleId { get; }

        public OldConsoleProcess(string consoleId)
        {
            ConsoleId = consoleId;
        }

        public async Task StartAsync()
        {
            var psi = new ProcessStartInfo
            {
                FileName = "OldConsoleApp.exe",
                Arguments = ConsoleId,
                UseShellExecute = true,
                CreateNoWindow = false
            };

            Process.Start(psi);
            await Task.CompletedTask;
        }

        public async Task<string> GetDataAsync()
        {
            try
            {
                using var client = new NamedPipeClientStream(".", ConsoleId, PipeDirection.InOut, PipeOptions.Asynchronous);
                await client.ConnectAsync(1000);
                using var reader = new StreamReader(client);
                var data = await reader.ReadLineAsync();
                return !string.IsNullOrEmpty(data) ? data : "No data";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data from {ConsoleId}: {ex.Message}");
                return "Error fetching data";
            }
        }
    }

    public static class ConsoleProcessFactory
    {
        public static IConsoleProcess CreateConsoleProcess(string type, string consoleId)
        {
            return type switch
            {
                "NewConsole" => new NewConsoleProcess(consoleId),
                "OldConsole" => new OldConsoleProcess(consoleId),
                _ => throw new ArgumentException("Invalid console type")
            };
        }
    }

    public class NamedPipeListener
    {
        private readonly string pipeName;
        private readonly IConsoleDataListener dataListener;

        public NamedPipeListener(string pipeName, IConsoleDataListener dataListener)
        {
            this.pipeName = pipeName;
            this.dataListener = dataListener;
        }

        public async Task StartListeningAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var server = new NamedPipeServerStream(pipeName, PipeDirection.In, NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Message, PipeOptions.Asynchronous);

                try
                {
                    await server.WaitForConnectionAsync(cancellationToken);

                    _ = Task.Run(async () =>
                    {
                        using (server)
                        {
                            try
                            {
                                using var reader = new StreamReader(server);
                                var message = await reader.ReadLineAsync();
                                if (!string.IsNullOrWhiteSpace(message))
                                {
                                    var parts = message.Split(':');
                                    if (parts.Length == 2)
                                    {
                                        string consoleId = parts[0];
                                        string value = parts[1];
                                        dataListener.OnDataReceived(consoleId, value);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in NamedPipeListener Task: {ex.Message}");
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in NamedPipeListener: {ex.Message}");
                }
            }
        }
    }

    public class OldConsolePoller
    {
        private readonly List<OldConsoleProcess> oldConsoles;
        private readonly IConsoleDataListener dataListener;

        public OldConsolePoller(List<OldConsoleProcess> oldConsoles, IConsoleDataListener dataListener)
        {
            this.oldConsoles = oldConsoles;
            this.dataListener = dataListener;
        }

        public void StartPolling()
        {
            foreach (var console in oldConsoles)
            {
                int pollingInterval = 5000;
                var timer = new Timer(async _ =>
                {
                    var data = await console.GetDataAsync();
                    dataListener.OnDataReceived(console.ConsoleId, data);
                }, null, 0, pollingInterval);
            }
        }
    }

    public class GameBoard : IConsoleDataListener
    {
        private readonly ConcurrentDictionary<string, string> consoleData = new();

        public void InitializeConsoleData(IEnumerable<string> consoleIds)
        {
            foreach (var id in consoleIds)
            {
                consoleData[id] = "Waiting for input...";
            }
        }

        public void OnDataReceived(string consoleId, string data)
        {
            consoleData[consoleId] = data;
        }

        public void Display()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("GameBoard Console:");
                foreach (var entry in consoleData)
                {
                    Console.WriteLine($"{entry.Key} - {entry.Value}");
                }
                Thread.Sleep(500);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int numberOfNewConsoles = GetConsoleCount("NewConsoles");
            int numberOfOldConsoles = GetConsoleCount("OldConsoles");

            var consoles = new List<IConsoleProcess>();
            var oldConsoleProcesses = new List<OldConsoleProcess>();
            var consoleIds = new List<string>();

            for (int i = 1; i <= numberOfNewConsoles; i++)
            {
                string consoleId = $"Console_{i}";
                var console = ConsoleProcessFactory.CreateConsoleProcess("NewConsole", consoleId);
                consoles.Add(console);
                consoleIds.Add(consoleId);
            }

            for (int i = 1; i <= numberOfOldConsoles; i++)
            {
                string consoleId = $"OldConsole_{i}";
                var console = (OldConsoleProcess)ConsoleProcessFactory.CreateConsoleProcess("OldConsole", consoleId);
                consoles.Add(console);
                oldConsoleProcesses.Add(console);
                consoleIds.Add(consoleId);
            }

            foreach (var console in consoles)
            {
                console.StartAsync().Wait();
                Console.WriteLine($"Started console with ID: {console.ConsoleId}");
            }

            Console.WriteLine($"Successfully started {numberOfNewConsoles} NewConsoles and {numberOfOldConsoles} OldConsoles.");

            var gameBoard = new GameBoard();
            gameBoard.InitializeConsoleData(consoleIds);

            var cancellationTokenSource = new CancellationTokenSource();

            var namedPipeListener = new NamedPipeListener("GameBoardPipe", gameBoard);
            _ = namedPipeListener.StartListeningAsync(cancellationTokenSource.Token);

            var oldConsolePoller = new OldConsolePoller(oldConsoleProcesses, gameBoard);
            oldConsolePoller.StartPolling();

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

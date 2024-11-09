using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace GameBoard
{
    class Program
    {
        static readonly ConcurrentDictionary<string, string> consoleData = new();

        static void Main(string[] args)
        {
            Console.Write("Enter the number of NewConsoles to start (1-100): ");
            var newConsoleInput = Console.ReadLine();

            Console.Write("Enter the number of OldConsoles to start (1-100): ");
            var oldConsoleInput = Console.ReadLine();

            if (!(int.TryParse(newConsoleInput, out int numberOfNewConsoles) ||
                (numberOfNewConsoles < 0 && numberOfNewConsoles >= 100)))
            {
                Console.WriteLine("Invalid Input for New Console, value must be less than 100");
            }

            if (!(int.TryParse(oldConsoleInput, out int numberOfOldConsoles) ||
                (numberOfOldConsoles < 0 && numberOfOldConsoles >= 100)))
            {
                Console.WriteLine("Invalid Input for Old Console, value must be less than 100");
            }

            StartConsoles("NewConsoleApp.exe", "Console_", numberOfNewConsoles);
            StartConsoles("OldConsoleApp.exe", "OldConsole_", numberOfOldConsoles);

            Console.WriteLine($"Successfully started {numberOfNewConsoles} " +
                $"NewConsoles and {numberOfOldConsoles} OldConsoles.");

            _ = Task.Run(() => ListenForUpdates());
            _ = Task.Run(() => StartPollingOldConsoles(numberOfOldConsoles));

            DisplayGameBoard();
        }

        static void StartConsoles(string executablePath, string prefix, int count)
        {
            for (int i = 1; i <= count; i++)
            {
                string uniqueId = $"{prefix}{i}";
                consoleData[uniqueId] = "Waiting for input...";

                ProcessStartInfo psi = new ()
                {
                    FileName = executablePath,
                    Arguments = uniqueId,
                    UseShellExecute = true,
                    CreateNoWindow = false
                };

                Process.Start(psi);
                Console.WriteLine($"Started {prefix} with ID: {uniqueId}");
            }
        }

        static async Task ListenForUpdates()
        {
            while (true)
            {
                using (var server =
                    new NamedPipeServerStream ("GameBoardPipe", PipeDirection.In, 
                    NamedPipeServerStream.MaxAllowedServerInstances,
                    PipeTransmissionMode.Message, PipeOptions.Asynchronous))
                {
                    try
                    {
                        await server.WaitForConnectionAsync();
                        using var reader = new StreamReader(server);
                        var message = await reader.ReadLineAsync();
                        if (!string.IsNullOrWhiteSpace(message))
                        {
                            var parts = message.Split(':');
                            if (parts.Length == 2)
                            {
                                string consoleId = parts[0];
                                string value = parts[1];

                                // Update the dictionary with the new value
                                consoleData[consoleId] = value;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in ListenForUpdates: {ex.Message}");
                    }
                }
            }
        }

        static void StartPollingOldConsoles(int numberOfOldConsoles)
        {
            for (int i = 1; i <= numberOfOldConsoles; i++)
            {
                int consoleIndex = i; // Capture the current value of i

                // Create a separate timer for each console
                var consoleTimer = new Timer(async _ =>
                {
                    try
                    {
                        string consoleId = $"OldConsole_{consoleIndex}";
                        string value = await GetOldConsoleData(consoleId);

                        consoleData[consoleId] = value;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error polling {consoleIndex}: {ex.Message}");
                    }
                }, null, 0, 5000); 
            }
        }


        static async Task<string> GetOldConsoleData(string consoleId)
        {
            try
            {
                using (var client = new NamedPipeClientStream(".", consoleId, PipeDirection.InOut, PipeOptions.Asynchronous))
                {
                    await client.ConnectAsync(1000);
                    using (var reader = new StreamReader(client))
                    {
                        string data = await reader.ReadLineAsync();
                        return !string.IsNullOrEmpty(data) ? data : "No data";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data from {consoleId}: {ex.Message}");
                return "Error fetching data";
            }
        }

        static void DisplayGameBoard()
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
}

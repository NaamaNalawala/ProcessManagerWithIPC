using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using GameBoard.Interfaces;

namespace GameBoard.Models
{
    public class OldConsoleProcess : IConsoleProcess
    {
        public string ConsoleId { get; }
        public string Status { get; set; }
        private Process _process;
        private string _lastScore;

        public OldConsoleProcess(string consoleId)
        {
            ConsoleId = consoleId;
            Status = "Stopped";
            _lastScore = "0";
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

            _process = Process.Start(psi);
            Status = "Running";

            // Monitor the process
            _process.EnableRaisingEvents = true;
            _process.Exited += (sender, e) =>
            {
                Status = "Stopped";
            };
            await Task.CompletedTask;
        }

        public async Task<string> GetDataAsync()
        {
            if (Status == "Stopped") // Returning last score without any new updation as the console is killed
            {
                return _lastScore;
            }
            try
            {
                //Here, the main game board is acting like a client and requesting for data from the server
                using var client = new NamedPipeClientStream(".", ConsoleId, PipeDirection.InOut, PipeOptions.Asynchronous);
                await client.ConnectAsync(1000);
                using var reader = new StreamReader(client);
                var data = await reader.ReadLineAsync();
                if (!string.IsNullOrEmpty(data))
                {
                    _lastScore = data; // Keep Updating the last score
                    return data;
                }
                else
                {
                    return _lastScore;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data from {ConsoleId}: {ex.Message}");
                return _lastScore; //return last score even in case of failure as per the requirement
            }
        }
    }
}

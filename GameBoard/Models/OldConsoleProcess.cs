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
}

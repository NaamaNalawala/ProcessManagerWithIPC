using System.Diagnostics;
using System.Threading.Tasks;
using GameBoard.Interfaces;

namespace GameBoard.Models
{
    public class NewConsoleProcess : IConsoleProcess
    {
        public string ConsoleId { get; }
        public string Status { get; set; }
        private Process _process;

        public NewConsoleProcess(string consoleId)
        {
            ConsoleId = consoleId;
            Status = "Stopped";
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
    }
}

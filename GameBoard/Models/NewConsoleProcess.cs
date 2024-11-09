using System.Diagnostics;
using System.Threading.Tasks;
using GameBoard.Interfaces;

namespace GameBoard.Models
{
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
}

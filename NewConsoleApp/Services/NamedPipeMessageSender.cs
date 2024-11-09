using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using NewConsoleApp.Interfaces;

namespace NewConsoleApp.Services
{
    public class NamedPipeMessageSender : IMessageSender
    {
        private readonly string _pipeName;

        public NamedPipeMessageSender(string pipeName)
        {
            _pipeName = pipeName;
        }

        public async Task SendMessageAsync(string consoleId, string message)
        {
            try
            {
                using var client = new NamedPipeClientStream(".", _pipeName, PipeDirection.Out);
                await client.ConnectAsync();
                using var writer = new StreamWriter(client) { AutoFlush = true };
                await writer.WriteLineAsync($"{consoleId}:{message}");
            }
            catch (Exception ex)
            {
                Logger.Log($"Error sending message: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            // Clean up resources if necessary
            await Task.CompletedTask;
        }
    }
}

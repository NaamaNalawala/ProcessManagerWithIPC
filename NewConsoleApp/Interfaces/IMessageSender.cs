using System;
using System.Threading.Tasks;

namespace NewConsoleApp.Interfaces
{
    public interface IMessageSender : IAsyncDisposable
    {
        Task SendMessageAsync(string consoleId, string message);
    }
}

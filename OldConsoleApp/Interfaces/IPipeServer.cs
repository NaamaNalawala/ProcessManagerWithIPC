using System.Threading.Tasks;

namespace OldConsoleApp.Interfaces
{
    // Interface for the named pipe server
    public interface IPipeServer
    {
        Task StartAsync();
    }
}
